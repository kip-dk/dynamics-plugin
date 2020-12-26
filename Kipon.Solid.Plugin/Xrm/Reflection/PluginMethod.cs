namespace Kipon.Xrm.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Xrm.Sdk;

    public class PluginMethod
    {
        public class Cache
        {
            private Types Types;
            private readonly Dictionary<string, PluginMethod[]> cache = new Dictionary<string, PluginMethod[]>();

            private static readonly object locks = new object();

            private static readonly string[] IGNORE_METHODS = new string[]
            {
                nameof(BasePlugin.Equals),
                nameof(BasePlugin.Execute),
                nameof(BasePlugin.GetHashCode),
                nameof(BasePlugin.GetType),
                nameof(BasePlugin.PluginMethodCache),
                nameof(BasePlugin.ReferenceEquals),
                nameof(BasePlugin.SecureConfig),
                nameof(BasePlugin.ToString),
                nameof(BasePlugin.UnsecureConfig)
            };

            public Cache(System.Reflection.Assembly assm)
            {
                this.Types = Types.Instance;
                this.Types.SetAssembly(assm);
            }

            public PluginMethod[] ForPlugin(Type type, int stage, string message, string primaryEntityName, bool isAsync, bool throwIfEmpty = true)
            {
                var key = type.FullName + "|" + stage + "|" + message + "|" + primaryEntityName + "|" + isAsync.ToString();

                if (cache.ContainsKey(key))
                {
                    return cache[key];
                }

                lock (locks)
                {
                    if (cache.ContainsKey(key))
                    {
                        return cache[key];
                    }

                    var lookFor = $"On{stage.ToStage()}{message}{(isAsync ? "Async" : "")}";
                    var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                    methods = (from m in methods where !m.Name.StartsWith("get_") && !IGNORE_METHODS.Contains(m.Name) select m).ToArray();

                    #region special handling of virtual entity plugin, that is the only thing than can end up calling in stage 30
                    if (stage == 30)
                    {
                        methods = methods.Where(r => r.Name == $"On{message}").ToArray();

                        MethodInfo fallback = null;
                        MethodInfo match = null;

                        foreach (var m in methods)
                        {
                            var attr = m.GetCustomAttributes(Types.LogicalNameAttribute).FirstOrDefault();
                            if (attr == null && fallback == null)
                            {
                                fallback = m;
                                continue;
                            }
                            if (attr == null && fallback != null)
                            {
                                throw new InvalidPluginExecutionException($"Virtaul entity plugin can only have a single method On{message} without explcit LogicalName attribute");
                            }

                            var logicalname = (string)attr.GetType().GetProperty("Value").GetValue(attr);
                            if (logicalname == primaryEntityName)
                            {
                                if (match == null)
                                {
                                    match = m;
                                    continue;
                                }

                                if (match != null)
                                {
                                    throw new InvalidPluginExecutionException($"There is more than one method On{message} that match logical name {primaryEntityName}. That is not allowed.");
                                }
                            }
                        }

                        if (match != null)
                        {
                            fallback = match;
                        }

                        if (fallback == null)
                        {
                            throw new InvalidPluginExecutionException($"Virtual Entity plugin must have a method called On{message}.");
                        }

                        var next = CreateFrom(fallback, null);

                        cache[key] = new[] { next };
                        return cache[key];
                    }
                    #endregion

                    var stepStage = stage == 40 && isAsync ? 41 : stage;
                    List<PluginMethod> results = new List<PluginMethod>();

                    foreach (var method in methods)
                    {
                        #region filter on logicalname attribute on method
                        if (primaryEntityName != null)
                        {
                            var logAttrs = method.GetCustomAttributes(Types.LogicalNameAttribute, false).ToArray();
                            if (logAttrs != null && logAttrs.Length > 0)
                            {
                                var foundLogicalName = false;
                                foreach (var logAttr in logAttrs)
                                {
                                    var name = (string)logAttr.GetType().GetProperty("Value").GetGetMethod().Invoke(logAttr, null);
                                    if (name == primaryEntityName)
                                    {
                                        foundLogicalName = true;
                                        break;
                                    }
                                }

                                if (!foundLogicalName)
                                {
                                    continue;
                                }
                            }
                        }
                        #endregion

                        #region explicit step decoration mathing
                        var cas = method.GetCustomAttributes(Types.StepAttribute, false);
                        var found = false;
                        foreach (var ca in cas)
                        {
                            var at = ca.GetType();
                            var _stage = (int)at.GetProperty("Stage").GetValue(ca);
                            var _message = at.GetProperty("Message").GetValue(ca).ToString();
                            var _primaryEntityName = (string)at.GetProperty("PrimaryEntityName").GetValue(ca);
                            var _isAsync = (bool)at.GetProperty("IsAsync").GetValue(ca);


                            if (_stage == stepStage && _message == message && _primaryEntityName == primaryEntityName && _isAsync == isAsync)
                            {
                                var next = CreateFrom(method, primaryEntityName);
                                AddIfConsistent(type, method, results, next, message, stage);
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            continue;
                        }

                        if (cas.Length > 0)
                        {
                            continue;
                        }
                        #endregion

                        #region find by naming convention
                        if (method.Name == lookFor)
                        {
                            var next = CreateFrom(method, primaryEntityName);

                            if (primaryEntityName == null)
                            {
                                AddIfConsistent(type, method, results, next, message, stage);
                                found = true;
                                continue;
                            }

                            var notrelevant = (from n in next.Parameters
                                               where n.LogicalName != null
                                                 && n.LogicalName != primaryEntityName
                                                 && n.IsGenericEntityInterface == false
                                               select n).Any();

                            if (notrelevant)
                            {
                                // at least on parameter is explicity something else than the current logical name
                                // so it is not relevant.
                                continue;
                            }

                            var logicalnames = (from l in next.Parameters
                                                where l.LogicalName != null
                                                  && l.IsGenericEntityInterface == false
                                                select l.LogicalName).Distinct().ToArray();

                            if (logicalnames.Length > 1)
                            {
                                throw new Exceptions.MultipleLogicalNamesException(type, method, logicalnames);
                            }

                            var logicalNames = (from n in next.Parameters where n.LogicalName != null select n.LogicalName).Distinct().ToArray();

                            if (logicalNames.Length == 1)
                            {
                                if (logicalNames[0] == primaryEntityName)
                                {
                                    AddIfConsistent(type, method, results, next, message, stage);
                                    found = true;
                                }
                                continue;
                            }

                            {
                                var logicalNameAttrs = method.GetCustomAttributes(Types.LogicalNameAttribute, false).ToArray();
                                foreach (var attr in logicalNameAttrs)
                                {
                                    var name = (string)attr.GetType().GetProperty("Value").GetValue(attr);
                                    if (name == primaryEntityName)
                                    {
                                        AddIfConsistent(type, method, results, next, message, stage);
                                        found = true;
                                        break;
                                    }
                                }

                                if (logicalNameAttrs.Length > 0)
                                {
                                    continue;
                                }
                            }

                            if (next.HasTargetPreOrPost() || next.HasTargetReference())
                            {
                                var logicalNamesAttrs = method.GetCustomAttributes(Types.LogicalNameAttribute, false).ToArray();
                                foreach (var attr in logicalNamesAttrs)
                                {
                                    var v = (string)attr.GetType().GetProperty("primaryEntityName").GetValue(attr);
                                    if (v == primaryEntityName)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (found)
                                {
                                    AddIfConsistent(type, method, results, next, message, stage);
                                    continue;
                                }
                            }
                            else
                            {
                                // TO-DO: complete the list of messages not related to a specific entity.
                                if (!IsKnownMessage(message))
                                {
                                    throw new NotImplementedException("handling method not attached to a logicalname is not supported yet.");
                                }
                            }
                        }
                        #endregion
                    }

                    if (results.Count == 0 && throwIfEmpty)
                    {
                        throw new Exceptions.UnresolvablePluginMethodException(type);
                    }

                    cache[key] = results.OrderBy(r => r.Sort).ToArray();
                    return cache[key];
                }
            }

            private bool IsKnownMessage(string message)
            {
                switch (message)
                {
                    case "RemoveMember": return true;
                }

                if (!message.StartsWith("_") && message.Contains("_")) return true;


                return false;
            }

            private void AddIfConsistent(Type type, System.Reflection.MethodInfo method, List<PluginMethod> results, PluginMethod result, string message, int stage)
            {
                #region validate pre and post image consistancy
                switch (stage)
                {
                    case 10:
                    case 20:
                        /* pre image pre event */
                        if (result.HasPreimageThatIsNotMergedImage() && message == "Create")
                        {
                            throw new Exceptions.UnavailableImageException(type, method, "Preimage", stage, message);
                        }
                        /* post image pre event */
                        if (result.HasPostimage())
                        {
                            throw new Exceptions.UnavailableImageException(type, method, "Postimage", stage, message);
                        }
                        break;
                    case 40:
                    case 41:
                        if (result.HasPostimage() && message == "Delete")
                        {
                            throw new Exceptions.UnavailableImageException(type, method, "Postimage", stage, message);
                        }
                        break;
                }
                #endregion

                #region validate target consistancy
                if (result.HasTargetBesideReference() && message == "Delete")
                {
                    throw new Exceptions.UnavailableImageException(type, method, "Target", stage, message);
                }

                if (result.HasTargetReference())
                {
                    var inError = true;
                    if (message == "Delete")
                    {
                        // delete always provides an entity reference as target, everything is good
                        inError = false;
                    }

                    var mess = message.Split('_');
                    if (mess.Length >= 2)
                    {
                        // this looks a lot like a custom action, it has the pattern prefix_name, bounded actions will also provide an entity reference, so we are good
                        inError = false;
                    }

                    // ADD other conditions where target is relevant, ex associate

                    if (inError)
                    {
                        throw new Exceptions.UnavailableImageException(type, method, "Target", stage, message);
                    }
                }
                #endregion

                results.Add(result);

                if (result.method.ReturnType != null)
                {
                    var outputProperties = result.method.ReturnType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (outputProperties != null && outputProperties.Length > 0)
                    {
                        var output = new Dictionary<System.Reflection.PropertyInfo, Output>();
                        foreach (var p in outputProperties)
                        {
                            var outputAttr = p.GetCustomAttribute(Types.OutputAttribute);
                            if (outputAttr != null)
                            {
                                var name = (string)outputAttr.GetType().GetProperty("LogicalName").GetValue(outputAttr);
                                var req = (bool)outputAttr.GetType().GetProperty("Required").GetValue(outputAttr);
                                output.Add(p, new Output { LogicalName = name, Requred = req });
                            }
                        }
                        if (output.Count > 0)
                        {
                            result.OutputProperties = output;
                        }
                    }
                }
            }

            private PluginMethod CreateFrom(System.Reflection.MethodInfo method, string logicalname)
            {
                var result = new PluginMethod();
                result.method = method;
                var parameters = method.GetParameters().ToArray();

                result.Parameters = new TypeCache[parameters.Length];
                var ix = 0;
                foreach (var parameter in parameters)
                {
                    result.Parameters[ix] = TypeCache.ForParameter(parameter, logicalname);
                    ix++;
                }

                var sortAttr = method.GetCustomAttributes(Types.SortAttribute, false).SingleOrDefault();
                if (sortAttr != null)
                {
                    result.Sort = (int)sortAttr.GetType().GetProperty("Value").GetValue(sortAttr);
                }
                else
                {
                    result.Sort = 1;
                }
                return result;
            }

        }

        private PluginMethod()
        {
        }

        public static string ImageSuffixFor(int pre1post2, int stage, bool async)
        {
            var first = "Preimage";
            if (pre1post2 == 2)
            {
                first = "Postimage";
            }
            switch (stage)
            {
                case 10: return $"{first}Validate";
                case 20: return $"{first}Pre";
                case 40:
                    {
                        if (async) return $"{first}PostAsync";
                        return $"{first}Post";
                    }
            }
            throw new ArgumentException($"{nameof(stage)} can be 10, 20 or 40");
        }

        private System.Reflection.MethodInfo method;
        public int Sort { get; set; }
        public TypeCache[] Parameters { get; private set; }

        public Dictionary<System.Reflection.PropertyInfo, Output> OutputProperties;

        #region target filter attributes
        private bool? _filterAllProperties;
        public bool FilterAllProperties
        {
            get
            {
                if (_filterAllProperties == null)
                {
                    _filterAllProperties = this.Parameters != null && this.Parameters.Where(r => r.IsTarget && r.AllProperties).Any();
                }
                return _filterAllProperties.Value;
            }
        }

        private CommonProperty[] _filteredProperties;
        public CommonProperty[] FilteredProperties
        {
            get
            {
                if (_filteredProperties == null)
                {
                    if (Parameters != null)
                    {
                        var result = new List<CommonProperty>();
                        foreach (var p in Parameters)
                        {
                            if (p.IsTarget && p.FilteredProperties != null && p.FilteredProperties.Length > 0)
                            {
                                result.AddRange(p.FilteredProperties);
                            }

                            if (p.IsPreimage && p.TargetFilterProperties.Length > 0)
                            {
                                result.AddRange(p.TargetFilterProperties);
                            }

                            if (p.IsPostimage && p.TargetFilterProperties.Length > 0)
                            {
                                result.AddRange(p.TargetFilterProperties);
                            }

                            if (p.IsMergedimage && p.TargetFilterProperties.Length > 0)
                            {
                                result.AddRange(p.TargetFilterProperties);
                            }

                        }
                        _filteredProperties = result.ToArray();
                    } else
                    {
                        _filteredProperties = new CommonProperty[0];
                    }
                }
                return _filteredProperties;
            }
        }
        #endregion

        #region preimage attributes
        private bool? _needPreimage;
        public bool NeedPreimage
        {
            get
            {
                if (this._needPreimage == null)
                {
                    this._needPreimage = this.Parameters != null && this.Parameters.Where(r => r.IsPreimage || r.IsMergedimage).Any();
                }
                return this._needPreimage.Value;
            }
        }

        private bool? _allPreimageProperties;
        public bool AllPreimageProperties
        {
            get
            {
                if (_allPreimageProperties == null)
                {
                    _allPreimageProperties = this.Parameters != null && this.Parameters.Where(r => (r.IsPreimage || r.IsMergedimage) && r.AllProperties).Any();
                }
                return _allPreimageProperties.Value;
            }
        }

        private CommonProperty[] _preimageProperties;
        public CommonProperty[] PreimageProperties
        {
            get
            {
                if (_preimageProperties == null)
                {
                    if (this.Parameters != null)
                    {
                        var result = new List<CommonProperty>();
                        foreach (var p in Parameters)
                        {
                            if ((p.IsPreimage || p.IsMergedimage) && p.FilteredProperties != null && p.FilteredProperties.Length > 0)
                            {
                                result.AddRange(p.FilteredProperties);
                            }
                        }
                        _preimageProperties = result.ToArray();
                    }
                    else
                    {
                        this._preimageProperties = new CommonProperty[0];
                    }
                }
                return _preimageProperties;
            }
        }
        #endregion

        #region postimage attributes
        private bool? _needPostimage;
        public bool NeedPostimage
        {
            get
            {
                if (this._needPostimage == null)
                {
                    this._needPostimage = this.Parameters != null && this.Parameters.Where(r => r.IsPostimage).Any();
                }
                return this._needPostimage.Value;
            }
        }

        private bool? _allPostimageProperties;
        public bool AllPostimageProperties
        {
            get
            {
                if (this._allPostimageProperties == null)
                {
                    this._allPostimageProperties = this.Parameters != null && this.Parameters.Where(r => r.IsPostimage && r.AllProperties).Any();
                }
                return this._allPostimageProperties.Value;
            }
        }

        private CommonProperty[] _postimageProperties;
        public CommonProperty[] PostimageProperties
        {
            get
            {
                if (this._postimageProperties == null)
                {
                    if (this.Parameters != null)
                    {
                        var result = new List<CommonProperty>();
                        foreach (var p in this.Parameters)
                        {
                            if (p.IsPostimage && p.FilteredProperties != null && p.FilteredProperties.Length > 0)
                            {
                                result.AddRange(p.FilteredProperties);
                            }
                        }
                        this._postimageProperties = result.ToArray();
                    }
                    else
                    {
                        this._postimageProperties = new CommonProperty[0];
                    }
                }
                return this._postimageProperties;
            }
        }
        #endregion

        private bool? _hasRequredProperties = null;
        public bool HasRequiredProperties
        {
            get
            {
                if (this._hasRequredProperties == null)
                {
                    this._hasRequredProperties = (from f in this.FilteredProperties where f.Required == true select f).Any();
                }
                return this._hasRequredProperties.Value;
            }
        }

        public bool HasPreimage()
        {
            return this.Parameters != null && (this.Parameters.Where(r => r.IsPreimage || r.IsMergedimage)).Any();
        }

        public bool HasPreimageThatIsNotMergedImage()
        {
            return this.Parameters != null && (this.Parameters.Where(r => r.IsPreimage)).Any();
        }

        public object Invoke(object instance, object[] args)
        {
            return this.method.Invoke(instance, args);
        }

        public bool HasPostimage()
        {
            return this.Parameters != null && (this.Parameters.Where(r => r.IsPostimage)).Any();
        }

        public bool HasTargetBesideReference()
        {
            return this.Parameters != null && (this.Parameters.Where(r => r.IsTarget && r.IsReference == false)).Any();
        }


        public bool HasTarget()
        {
            return this.Parameters != null && (this.Parameters.Where(r => r.IsTarget)).Any();
        }

        public bool HasTargetPreOrPost()
        {
            return this.Parameters != null && this.Parameters.Where(r => r.IsTarget || r.IsPreimage || r.IsMergedimage || r.IsPostimage).Any();
        }

        public bool HasTargetReference()
        {
            return this.Parameters != null && this.Parameters.Where(r => r.IsReference).Any();
        }

        public bool IsRelevant(Microsoft.Xrm.Sdk.Entity target)
        {
            if (this.FilterAllProperties)
            {
                return true;
            }

            if (this.FilteredProperties != null && this.FilteredProperties.Length > 0)
            {
                foreach (var f in this.FilteredProperties)
                {
                    if (target.Attributes.Keys.Contains(f.LogicalName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    internal static class PluginMethodCacheLocalExtensions
    {
        public static string ToStage(this int value)
        {
            switch(value)
            {
                case 10: return "Validate";
                case 20: return "Pre";
                case 30: return ""; // virtual entity query request
                case 40: return "Post";
                default: throw new Microsoft.Xrm.Sdk.InvalidPluginExecutionException($"Unknown state {value}");
            }
        }
    }
}
