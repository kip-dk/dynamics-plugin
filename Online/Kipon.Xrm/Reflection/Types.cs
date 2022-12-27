namespace Kipon.Xrm.Reflection
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public sealed class Types
    {
        public static readonly string[] MESSAGE_WITHOUT_PRIMARY_ENTITY = new string[]
        {
            "AddMember",
            "RemoveMember",
            "Associate",
            "Disassociate"
        };

        private Dictionary<string, Type> entityTypes = new Dictionary<string, Type>();

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

            this.TargetAttribute = typeof(Kipon.Xrm.Attributes.TargetAttribute);
            this.TargetFilterAttribute = typeof(Kipon.Xrm.Attributes.TargetFilterAttribute);
            this.PreimageAttribute = typeof(Kipon.Xrm.Attributes.PreimageAttribute);
            this.MergedimageAttribute = typeof(Kipon.Xrm.Attributes.MergedimageAttribute);
            this.PostimageAttribute = typeof(Kipon.Xrm.Attributes.PostimageAttribute);
            this.AdminAttribute = typeof(Kipon.Xrm.Attributes.AdminAttribute);
            this.ExportAttribute = typeof(Kipon.Xrm.Attributes.ExportAttribute);
            this.ImportingConstructorAttribute = typeof(Kipon.Xrm.Attributes.ImportingConstructorAttribute);
            this.RequiredAttribute = typeof(Kipon.Xrm.Attributes.RequiredAttribute);
            this.StepAttribute = typeof(Kipon.Xrm.Attributes.StepAttribute);
            this.LogicalNameAttribute = typeof(Kipon.Xrm.Attributes.LogicalNameAttribute);
            this.SortAttribute = typeof(Kipon.Xrm.Attributes.SortAttribute);
            this.OutputAttribute = typeof(Kipon.Xrm.Attributes.OutputAttribute);

            this.IfAttribute = typeof(Kipon.Xrm.Attributes.IfAttribute);

            this.AbstractActionRequest = typeof(Kipon.Xrm.Actions.AbstractActionRequest);

            this.ITarget = typeof(Kipon.Xrm.ITarget);
            this.Target = typeof(Kipon.Xrm.Target<>);
            this.TargetReference = typeof(Kipon.Xrm.TargetReference<>);
            this.Preimage = typeof(Kipon.Xrm.Preimage<>);
            this.Mergedimage = typeof(Kipon.Xrm.Mergedimage<>);
            this.Postimage = typeof(Kipon.Xrm.Postimage<>);
            this.ActionTarget = typeof(Kipon.Xrm.ActionTarget<>);

            this.IUnitOfWork = typeof(Kipon.Xrm.IUnitOfWork);
            this.IAdminUnitOfWork = typeof(Kipon.Xrm.IAdminUnitOfWork);
            this.IEntityCache = typeof(Kipon.Xrm.ServiceAPI.IEntityCache);

            this.IRepository = typeof(Kipon.Xrm.IRepository<>);

            this.BasePlugin = typeof(Kipon.Xrm.BasePlugin);
            this.VirtualEntityPlugin = typeof(Kipon.Xrm.VirtualEntityPlugin);

            this.IPluginContext = typeof(Kipon.Xrm.IPluginContext);

            var initializer = typeof(Kipon.Xrm.ServiceAPI.IStaticInitializer);
            var allTypes = assembly.GetTypes();

            foreach (var type in allTypes)
            {
                if (type.Implements(initializer))
                {
                    var init = (Kipon.Xrm.ServiceAPI.IStaticInitializer)Activator.CreateInstance(type);
                    init.Initialize();
                }
            }
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
        public Type OutputAttribute { get; private set; }
        public Type ITarget { get; private set; }
        public Type Target { get; private set; }
        public Type TargetReference { get; private set; }
        public Type TargetFilterAttribute { get; private set; }
        public Type Preimage { get; private set; }
        public Type Mergedimage { get; private set; }
        public Type Postimage { get; private set; }
        public Type ActionTarget { get; private set; }
        public Type IUnitOfWork { get; private set; }
        public Type IAdminUnitOfWork { get; private set; }
        public Type IRepository { get; private set; }

        public Type AbstractActionRequest { get; private set; }

        public Type IEntityCache { get; private set; }

        public System.Reflection.Assembly Assembly { get; private set; }


        public Type BasePlugin { get; private set; }
        public Type VirtualEntityPlugin { get; private set; }

        public Type IPluginContext { get; private set; }

        public Type IfAttribute { get; private set; }

        public Type EntityTypeFor(string logicalname)
        {
            if (entityTypes.ContainsKey(logicalname))
            {
                return entityTypes[logicalname];
            }

            if (this.Assembly != null)
            {

                var allTypes = this.Assembly.GetTypes().Where(r => r.BaseType == typeof(Microsoft.Xrm.Sdk.Entity));
                foreach (var type in allTypes)
                {
                    var name = (Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute)type.GetCustomAttributes(typeof(Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute), false).SingleOrDefault();
                    if (name != null)
                    {
                        entityTypes[name.LogicalName] = type;
                    }
                }

                if (entityTypes.ContainsKey(logicalname))
                {
                    return entityTypes[logicalname];
                }
            }
            throw new Exceptions.UnknownEntityTypeException(logicalname);
        }

        public Type TypeForInterface(Type intf)
        {
            return this.Assembly.GetTypes().Where(t => t.Implements(intf)).Single();
        } 

    }
}
