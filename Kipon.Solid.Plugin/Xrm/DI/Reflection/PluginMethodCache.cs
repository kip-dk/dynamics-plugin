using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.DI.Reflection
{
    public class PluginMethodCache
    {
        private static readonly Dictionary<string, PluginMethodCache[]> cache = new Dictionary<string, PluginMethodCache[]>();

        private PluginMethodCache()
        {
        }

        public static PluginMethodCache[] ForPlugin(Type type, int stage, string message, string primaryEntityName, bool isAsync)
        {
            var key = type.FullName + "|" + stage + "|" + message + "|" + primaryEntityName + "|" + isAsync.ToString();

            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var lookFor = $"On{stage.ToStage()}{message}{(isAsync?"Async":"")}";

            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var stepStage = stage == 40 && isAsync ? 41 : stage;

            List<PluginMethodCache> results = new List<PluginMethodCache>();

            foreach (var method in methods)
            {
                #region explicit step decoration mathing
                var cas = method.GetCustomAttributes(typeof(Attributes.StepAttribute), false);
                var found = false;
                foreach (Attributes.StepAttribute ca in cas)
                {
                    if ((int)ca.Stage == stepStage && ca.Message.ToString() == message && ca.PrimaryEntityName == primaryEntityName && ca.IsAsync == isAsync)
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

                    if (next.HasTargetPreOrPost())
                    {
                        var logicalNamesAttrs = method.GetCustomAttributes(typeof(Kipon.Xrm.Attributes.LogicalNameAttribute), false).ToArray();
                        foreach (Kipon.Xrm.Attributes.LogicalNameAttribute attr in logicalNamesAttrs)
                        {
                            if (attr.Value == primaryEntityName)
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
                        throw new NotImplementedException("handling method not attached to an logicalname is not supported yet.");
                    }
                }
                #endregion
            }

            if (results.Count == 0)
            {
                throw new Exceptions.UnresolvablePluginMethodException(type);
            }

            cache[key] = results.OrderBy(r => r.Sort).ToArray();
            return cache[key];
        }

        private static void AddIfConsistent(Type type, System.Reflection.MethodInfo method, List<PluginMethodCache> results, PluginMethodCache result, string message, int stage)
        {
            #region validate pre and post image consistancy
            switch ((Kipon.Xrm.Attributes.StepAttribute.StageEnum)stage)
            {
                case Attributes.StepAttribute.StageEnum.Validate:
                case Attributes.StepAttribute.StageEnum.Pre:
                    /* pre image pre event */
                    if (result.HasPreimage() && message == Kipon.Xrm.Attributes.StepAttribute.MessageEnum.Create.ToString())
                    {
                        throw new Exceptions.UnavailableImageException(type, method, "Preimage", stage, message);
                    }
                    /* post image pre event */
                    if (result.HasPostimage())
                    {
                        throw new Exceptions.UnavailableImageException(type, method, "Postimage", stage, message);
                    }
                    break;
                case Attributes.StepAttribute.StageEnum.Post:
                case Attributes.StepAttribute.StageEnum.PostAsync:
                    if (result.HasPostimage() && message == Kipon.Xrm.Attributes.StepAttribute.MessageEnum.Delete.ToString())
                    {
                        throw new Exceptions.UnavailableImageException(type, method, "Postimage", stage, message);
                    }
                    break;
            }
            #endregion

            #region validate target consistancy
            if (result.HasTarget() && message == Kipon.Xrm.Attributes.StepAttribute.MessageEnum.Delete.ToString())
            {
                throw new Exceptions.UnavailableImageException(type, method, "Target", stage, message);
            }
            #endregion

            results.Add(result);
        }

        private static PluginMethodCache CreateFrom(System.Reflection.MethodInfo method)
        {
            var result = new PluginMethodCache();
            result.method = method;
            var parameters = method.GetParameters().DefaultIfEmpty().ToArray();

            result.Parameters = new TypeCache[parameters.Length];
            var ix = 0;
            foreach (var parameter in parameters)
            {
                result.Parameters[ix] = TypeCache.ForParameter(parameter);
                ix++;
            }

            var sortAttr = (Attributes.SortAttribute)method.GetCustomAttributes(typeof(Attributes.SortAttribute), false).SingleOrDefault();
            if (sortAttr != null)
            {
                result.Sort = sortAttr.Value;
            } else
            {
                result.Sort = 1;
            }
            return result;
        }


        private System.Reflection.MethodInfo method;
        public int Sort { get; set; }
        public TypeCache[] Parameters { get; private set; }

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

        private CommonPropertyCache[] _filteredProperties;
        public CommonPropertyCache[] FilteredProperties
        {
            get
            {
                if (_filteredProperties == null)
                {
                    if (Parameters != null)
                    {
                        var result = new List<CommonPropertyCache>();
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
                        _filteredProperties = new CommonPropertyCache[0];
                    }
                }
                return _filteredProperties;
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
