using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Models
{
    public class Plugin
    {
        private List<Step> steps = new List<Step>();

        public Plugin(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
        public Step[] Steps => this.steps.ToArray();

        public Entities.PluginType CurrentCrmInstance { get; set; }

        internal void AddStep(int stage, string message, string logicalname, bool isAsync, Kipon.Tools.Xrm.Reflection.PluginMethod[] methods)
        {
            if (methods == null || methods.Length == 0)
            {
                return;
            }
            var next = new Step
            {
                Stage = stage,
                Message = message,
                PrimaryEntityLogicalName = logicalname,
                IsAsync = isAsync,
                ExecutionOrder = methods.Min(r => r.Sort)
            };

            if (message.ToLower() == "update")
            {
                next.FilteringAttributes = methods.FilteringAttributes();
            }

            if (message.ToLower() != "create")
            {
                next.PreImage = methods.Preimage();
            }

            if (stage == 40)
            {
                next.PostImage = methods.Postimage();
            }

            steps.Add(next);
        }

        public string NameOf(int stage, string message, bool async, string logicalname)
        {
            return $"{this.Type.FullName} {StageName(stage, async)} {message} of {logicalname}";
        }

        private string StageName(int stage, bool async)
        {
            switch (stage)
            {
                case 10: return "validate";
                case 20: return "pre";
                case 40: return async ? "post async" : "post";
            }
            throw new NotImplementedException($"Stage  {stage} is not implemented");
        }
    }

    internal static class PluginLocalExtensions
    {
        public static string[] FilteringAttributes(this Kipon.Tools.Xrm.Reflection.PluginMethod[] methods)
        {
            var hasAll = (from m in methods where m.FilterAllProperties == true select m).Any();
            if (hasAll)
            {
                return null;
            }
            List<string> unique = new List<string>();
            foreach (var m in methods)
            {
                if (m.FilteredProperties != null && m.FilteredProperties.Length > 0)
                {
                    unique.AddRange(m.FilteredProperties.Select(r => r.LogicalName));
                }
            }
            if (unique.Count > 0)
            {
                return unique.Distinct().OrderBy(r => r).ToArray();
            }

            return null;
        }

        public static Image Preimage(this Kipon.Tools.Xrm.Reflection.PluginMethod[] methods)
        {
            var hasAll = (from m in methods where m.AllPreimageProperties == true select m).Any();
            if (hasAll)
            {
                return new Image
                {
                    AllAttributes = true
                };
            }

            var hasPreimage = (from m in methods where m.HasPreimage() select m).Any();
            if (!hasPreimage)
            {
                return null;
            }

            List<string> unique = new List<string>();
            foreach (var m in methods)
            {
                if (m.PreimageProperties != null && m.PreimageProperties.Length > 0)
                {
                    unique.AddRange(m.PreimageProperties.Select(r => r.LogicalName));
                }
            }

            return new Image
            {
                FilteredAttributes = unique.Distinct().OrderBy(r => r).ToArray()
            };
        }


        public static Image Postimage(this Kipon.Tools.Xrm.Reflection.PluginMethod[] methods)
        {
            var hasAll = (from m in methods where m.AllPostimageProperties == true select m).Any();
            if (hasAll)
            {
                return new Image
                {
                    AllAttributes = true
                };
            }

            var hasPostimage = (from m in methods where m.HasPostimage() select m).Any();
            if (!hasPostimage)
            {
                return null;
            }

            List<string> unique = new List<string>();
            foreach (var m in methods)
            {
                if (m.PostimageProperties != null && m.PostimageProperties.Length > 0)
                {
                    unique.AddRange(m.PostimageProperties.Select(r => r.LogicalName));
                }
            }

            return new Image
            {
                FilteredAttributes = unique.Distinct().OrderBy(r => r).ToArray()
            };
        }
    }
}
