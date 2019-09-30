using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Reflection
{
    public class Types
    {
        private static readonly Type _TargetAttribute = typeof(Kipon.Xrm.Attributes.TargetAttribute);
        private static readonly Type _PreimageAttribute = typeof(Kipon.Xrm.Attributes.PreimageAttribute);
        private static readonly Type _MergedimageAttribute = typeof(Kipon.Xrm.Attributes.MergedimageAttribute);
        private static readonly Type _PostimageAttribute = typeof(Kipon.Xrm.Attributes.MergedimageAttribute);

        private static readonly Type _Target = typeof(Kipon.Xrm.Target<>);
        private static readonly Type _TargetReference = typeof(Kipon.Xrm.TargetReference<>);
        private static readonly Type _Preimage = typeof(Kipon.Xrm.Preimage<>);
        private static readonly Type _Mergedimage = typeof(Kipon.Xrm.Mergedimage<>);
        private static readonly Type _Postimage = typeof(Kipon.Xrm.Postimage<>);

        private static readonly System.Reflection.Assembly _Assembly = typeof(Types).Assembly;

        public Types()
        {
        }

        public Type TargetAttribute => Types._TargetAttribute;
        public Type PreimageAttribute => Types._Preimage;
        public Type MergedimageAttribute => Types._MergedimageAttribute;
        public Type PostimageAttribute => Types._PostimageAttribute;
        public Type Target => Types._Target;
        public Type TargetReference => Types._TargetReference;
        public Type Preimage => Types._Preimage;
        public Type Mergedimage => Types._Mergedimage;
        public Type Postimage => Types._Postimage;
        public System.Reflection.Assembly Assembly => _Assembly;
    }


}
