namespace Kipon.Xrm.Reflection
{
    using System;
    using System.Linq;

    public sealed class Types
    {
        private const string NAMESPACE = "Kipon" + "." + "Xrm" + ".";

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

            this.TargetAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.TargetAttribute)}"];
            this.PreimageAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.PreimageAttribute)}"];
            this.MergedimageAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.MergedimageAttribute)}"];
            this.PostimageAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.PostimageAttribute)}"];
            this.AdminAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.AdminAttribute)}"];
            this.ExportAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.ExportAttribute)}"];
            this.ImportingConstructorAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.ImportingConstructorAttribute)}"];
            this.RequiredAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.RequiredAttribute)}"];
            this.StepAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.StepAttribute)}"];
            this.LogicalNameAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.LogicalNameAttribute)}"];
            this.SortAttribute = allTypes[$"{NAMESPACE}Attributes.{nameof(_instance.SortAttribute)}"];

            this.Target = allTypes[$"{NAMESPACE}{nameof(_instance.Target)}`1"];
            this.TargetReference = allTypes[$"{NAMESPACE}{nameof(_instance.TargetReference)}`1"];
            this.Preimage = allTypes[$"{NAMESPACE}{nameof(_instance.Preimage)}`1"];
            this.Mergedimage = allTypes[$"{NAMESPACE}{nameof(_instance.Mergedimage)}`1"];
            this.Postimage = allTypes[$"{NAMESPACE}{nameof(_instance.Postimage)}`1"];

            this.IUnitOfWork = allTypes[$"{NAMESPACE}{nameof(_instance.IUnitOfWork)}"];
            this.IAdminUnitOfWork = allTypes[$"{NAMESPACE}{nameof(_instance.IAdminUnitOfWork)}"];

            this.IRepository = allTypes[$"{NAMESPACE}{nameof(_instance.IRepository)}`1"];

            this.BasePlugin = allTypes[$"{NAMESPACE}{nameof(_instance.BasePlugin)}"];

            this.IPluginContext = allTypes[$"{NAMESPACE}{nameof(_instance.IPluginContext)}"];
        }

        public Type TargetAttribute { get; private set; }
        public Type PreimageAttribute { get; private set; }
        public Type MergedimageAttribute { get; private set; }
        public Type PostimageAttribute { get; private set; }
        public Type AdminAttribute { get; private set; }
        public Type ExportAttribute { get; private set; }
        public Type ImportingConstructorAttribute { get; private set; }
        public Type RequiredAttribute { get; private set; }
        public Type StepAttribute { get; private set; }
        public Type LogicalNameAttribute { get; private set; }
        public Type SortAttribute { get; private set; }

        public Type Target { get; private set; }
        public Type TargetReference { get; private set; }
        public Type Preimage { get; private set; }
        public Type Mergedimage { get; private set; }
        public Type Postimage { get; private set; }
        public Type IUnitOfWork { get; private set; }
        public Type IAdminUnitOfWork { get; private set; }

        public Type IRepository { get; private set; }

        public System.Reflection.Assembly Assembly { get; private set; }

        public Type BasePlugin { get; private set; }

        public Type IPluginContext { get; private set; }

    }
}
