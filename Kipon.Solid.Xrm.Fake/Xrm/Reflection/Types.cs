using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Reflection
{
    public sealed class Types
    {

        private static Types _instance;

        static Types()
        {
            _instance = new Types();
        }

        private Types()
        {
        }

        
        public static Types Instance
        {
            get
            {
                return _instance;
            }
        }

        public void SetAssembly(System.Reflection.Assembly assembly)
        {
            this.Assembly = assembly;
            var allTypes = assembly.GetTypes().ToDictionary(r => r.FullName);

            this.TargetAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.TargetAttribute)}"];
            this.PreimageAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.PreimageAttribute)}"];
            this.MergedimageAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.MergedimageAttribute)}"];
            this.PostimageAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.PostimageAttribute)}"];
            this.AdminAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.AdminAttribute)}"];
            this.ExportAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.ExportAttribute)}"];
            this.ImportingConstructorAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.ImportingConstructorAttribute)}"];
            this.RequiredAttribute = allTypes[$"Kipon.Xrm.Attributes.{nameof(_instance.RequiredAttribute)}"];

            this.Target = allTypes[$"Kipon.Xrm.{nameof(_instance.Target)}`1"];
            this.TargetReference = allTypes[$"Kipon.Xrm.{nameof(_instance.TargetReference)}`1"];
            this.Preimage = allTypes[$"Kipon.Xrm.{nameof(_instance.Preimage)}`1"];
            this.Mergedimage = allTypes[$"Kipon.Xrm.{nameof(_instance.Mergedimage)}`1"];
            this.Postimage = allTypes[$"Kipon.Xrm.{nameof(_instance.Postimage)}`1"];

            this.IUnitOfWork = allTypes[$"Kipon.Xrm.{nameof(_instance.IUnitOfWork)}"];
            this.IAdminUnitOfWork = allTypes[$"Kipon.Xrm.{nameof(_instance.IAdminUnitOfWork)}"];
        }

        public Type TargetAttribute { get; private set; }
        public Type PreimageAttribute { get; private set; }
        public Type MergedimageAttribute { get; private set; }
        public Type PostimageAttribute { get; private set; }
        public Type AdminAttribute { get; private set; }
        public Type ExportAttribute { get; private set; }
        public Type ImportingConstructorAttribute { get; private set; }
        public Type RequiredAttribute { get; private set; }
        public Type Target { get; private set; }
        public Type TargetReference { get; private set; }
        public Type Preimage { get; private set; }
        public Type Mergedimage { get; private set; }
        public Type Postimage { get; private set; }
        public Type IUnitOfWork { get; private set; }
        public Type IAdminUnitOfWork { get; private set; }

        public System.Reflection.Assembly Assembly { get; private set; }

    }
}
