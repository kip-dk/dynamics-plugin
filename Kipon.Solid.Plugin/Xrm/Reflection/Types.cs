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
        private static readonly Type _AdminAttribute = typeof(Kipon.Xrm.Attributes.AdminAttribute);
        private static readonly Type _ExportAttribute = typeof(Kipon.Xrm.Attributes.ExportAttribute);
        private static readonly Type _ImportingConstructorAttribute = typeof(Kipon.Xrm.Attributes.ImportingConstructorAttribute);
        private static readonly Type _RequiredAttribute = typeof(Kipon.Xrm.Attributes.RequiredAttribute);
        private static readonly Type _StepAttribute = typeof(Kipon.Xrm.Attributes.StepAttribute);
        private static readonly Type _LogicalNameAttribute = typeof(Kipon.Xrm.Attributes.LogicalNameAttribute);
        private static readonly Type _SortAttribute = typeof(Kipon.Xrm.Attributes.SortAttribute);


        private static readonly Type _Target = typeof(Kipon.Xrm.Target<>);
        private static readonly Type _TargetReference = typeof(Kipon.Xrm.TargetReference<>);
        private static readonly Type _Preimage = typeof(Kipon.Xrm.Preimage<>);
        private static readonly Type _Mergedimage = typeof(Kipon.Xrm.Mergedimage<>);
        private static readonly Type _Postimage = typeof(Kipon.Xrm.Postimage<>);
        private static readonly Type _IUnitOfWork = typeof(Kipon.Xrm.IUnitOfWork);
        private static readonly Type _IAdminUnitOfWork = typeof(Kipon.Xrm.IAdminUnitOfWork);
        private static readonly Type _IRepository = typeof(Kipon.Xrm.IRepository<>);

        private static readonly System.Reflection.Assembly _Assembly = typeof(Types).Assembly;

        private static readonly Types _instance = null;

        static Types()
        {
            _instance = new Types();
        }

        private Types()
        {
        }

        internal static Types Instance
        {
            get
            {
                return _instance;
            }
        }

        public Type TargetAttribute => Types._TargetAttribute;
        public Type PreimageAttribute => Types._PreimageAttribute;
        public Type MergedimageAttribute => Types._MergedimageAttribute;
        public Type PostimageAttribute => Types._PostimageAttribute;
        public Type AdminAttribute => Types._AdminAttribute;
        public Type ExportAttribute => Types._ExportAttribute;
        public Type ImportingConstructorAttribute => Types._ImportingConstructorAttribute;
        public Type RequiredAttribute => Types._RequiredAttribute;
        public Type StepAttribute => Types._StepAttribute;
        public Type LogicalNameAttribute => Types._LogicalNameAttribute;
        public Type SortAttribute => Types._SortAttribute;

        public Type Target => Types._Target;
        public Type TargetReference => Types._TargetReference;
        public Type Preimage => Types._Preimage;
        public Type Mergedimage => Types._Mergedimage;
        public Type Postimage => Types._Postimage;
        public Type IUnitOfWork => Types._IUnitOfWork;
        public Type IAdminUnitOfWork => Types._IAdminUnitOfWork;
        public Type IRepository => _IRepository;
        public System.Reflection.Assembly Assembly => _Assembly;
    }


}
