using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Reflection
{
    public class Types
    {
        public static readonly Type TargetAttribute = typeof(Kipon.Xrm.Attributes.TargetAttribute);
        public static readonly Type PreimageAttribute = typeof(Kipon.Xrm.Attributes.PreimageAttribute);
        public static readonly Type MergedimageAttribute = typeof(Kipon.Xrm.Attributes.MergedimageAttribute);
        public static readonly Type PostimageAttribute = typeof(Kipon.Xrm.Attributes.MergedimageAttribute);

        public static readonly Type Target = typeof(Kipon.Xrm.Target<>);
        public static readonly Type TargetReference = typeof(Kipon.Xrm.TargetReference<>);
        public static readonly Type Preimage = typeof(Kipon.Xrm.Preimage<>);
        public static readonly Type Mergedimage = typeof(Kipon.Xrm.Mergedimage<>);
        public static readonly Type Postimage = typeof(Kipon.Xrm.Postimage<>);

        public static readonly System.Reflection.Assembly Assembly = typeof(Types).Assembly;
    }
}
