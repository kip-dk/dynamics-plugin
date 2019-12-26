namespace Kipon.Xrm.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class PluginMethod
    {
        private static readonly Dictionary<string, PluginMethod[]> cache = new Dictionary<string, PluginMethod[]>();

        private static Types Types { get; set; }

        static PluginMethod()
        {
            PluginMethod.Types = Types.Instance;
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

        public static PluginMethod[] ForPlugin(Type type, int stage, string message, string primaryEntityName, bool isAsync, bool throwIfEmpty = true)
        {
            var key = type.FullName + "|" + stage + "|" + message + "|" + primaryEntityName + "|" + isAsync.ToString();

            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var lookFor = $"On{stage.ToStage()}{message}{(isAsync?"Async":"")}";

            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var stepStage = stage == 40 && isAsync ? 41 : stage;

            List<PluginMethod> results = new List<PluginMethod>();

            foreach (var method in methods)
            {
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
                        var next = CreateFrom(method);
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
                    var next = CreateFrom(method);
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

                    if (logicalNames.Length > 1)
                    {
                        throw new Exceptions.MultipleLogicalNamesException(type, method);
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
#warning TO-DO
                        // TO-DO: what to do, we have a method match, but no entities. Some methods ex. assosiate does not have target in deployment process.
                        throw new NotImplementedException("handling method not attached to a logicalname is not supported yet.");
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

        private static void AddIfConsistent(Type type, System.Reflection.MethodInfo method, List<PluginMethod> results, PluginMethod result, string message, int stage)
        {
            #region validate pre and post image consistancy
            switch (stage)
            {
                case 10:
                case 20:
                    /* pre image pre event */
                    if (result.HasPreimage() && message == "Create")
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
            if (result.HasTarget() && message == "Delete")
            {
                throw new Exceptions.UnavailableImageException(type, method, "Target", stage, message);
            }

            if (result.HasTargetReference())
            {
                var inError = true;
                if (message == "Delete")
                {
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
        }

        private static PluginMethod CreateFrom(System.Reflection.MethodInfo method)
        {
            var result = new PluginMethod();
            result.method = method;
            var parameters = method.GetParameters().DefaultIfEmpty().ToArray();

            result.Parameters = new TypeCache[parameters.Length];
            var ix = 0;
            foreach (var parameter in parameters)
            {
                result.Parameters[ix] = TypeCache.ForParameter(parameter);
                ix++;
            }

            var sortAttr = method.GetCustomAttributes(Types.SortAttribute, false).SingleOrDefault();
            if (sortAttr != null)
            {
                result.Sort = (int)sortAttr.GetType().GetProperty("Value").GetValue(sortAttr);
            } else
            {
                result.Sort = 1;
            }
            return result;
        }


        private System.Reflection.MethodInfo method;
        public int Sort { get; set; }
        public TypeCache[] Parameters { get; private set; }

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
                    this._allPostimageProperties = this.Parameters != null && this.Parameters.Where(r => r.AllProperties).Any();
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

        public void Invoke(object instance, object[] args)
        {
            this.method.Invoke(instance, args);
        }

        public bool HasPostimage()
        {
            return this.Parameters != null && (this.Parameters.Where(r => r.IsPostimage)).Any();
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
                case 40: return "Post";
                default: throw new Microsoft.Xrm.Sdk.InvalidPluginExecutionException($"Unknown state {value}");
            }
        }
    }
}
