using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Entities;

namespace Kipon.Xrm.Tools.Services
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ServiceAPI.IPluginAssemblyService))]
    public class PluginAssemblyService : ServiceAPI.IPluginAssemblyService
    {
        private Entities.IUnitOfWork uow;
        private ServiceAPI.IMessageService messageService;

        public System.Reflection.Assembly Assembly { get; private set; }
        private Entities.PluginAssembly pluginAssembly;
        private byte[] code;
        private bool isNew = true;

        [ImportingConstructor]
        public PluginAssemblyService(Entities.IUnitOfWork uow, ServiceAPI.IMessageService messageService)
        {
            this.uow = uow;
            this.messageService = messageService;
        }

        public PluginAssembly FindOrCreate(string assemblyfilename)
        {
            this.code = System.IO.File.ReadAllBytes(assemblyfilename);
            this.Assembly = System.Reflection.Assembly.Load(code);
            var publickeytoken = this.GetPublicKeyTokenFromAssembly();

            var name = assemblyfilename.Split(new char[] { '\\', '/' }).Last();
            if (name.ToUpper().EndsWith(".DLL"))
            {
                name = name.Substring(0, name.Length - 4);
            }

            var r = (from p in uow.PluginAssemblies.GetQuery()
                    where p.Name == name
                    select p).SingleOrDefault();
            if (r != null)
            {
                this.pluginAssembly = r;
                this.isNew = false;
                return r;
            }

            r = new PluginAssembly
            {
                PluginAssemblyId = Guid.NewGuid(),
                Content = System.Convert.ToBase64String(code),
                Description = name,
                IsolationMode = new Microsoft.Xrm.Sdk.OptionSetValue(2),
                Name = name,
                SourceType = new Microsoft.Xrm.Sdk.OptionSetValue(0),
                Culture = "neutral",
                PublicKeyToken = publickeytoken,
                Version = "1.0"
            };
            uow.Create(r);
            this.messageService.Inform("Assembly code was created");

            this.pluginAssembly = r;
            this.isNew = true;

            return r;
        }


        public void UploadAssembly()
        {
            if (!isNew)
            {
                var clean = new Entities.PluginAssembly { PluginAssemblyId = this.pluginAssembly.PluginAssemblyId };
                clean.Content = System.Convert.ToBase64String(code);
                uow.Update(clean);
                this.messageService.Inform("Assembly code updated");
            }
        }


        private string GetPublicKeyTokenFromAssembly()
        {
            var bytes = this.Assembly.GetName().GetPublicKeyToken();
            if (bytes == null || bytes.Length == 0)
                return "None";

            var publicKeyToken = string.Empty;
            for (int i = 0; i < bytes.GetLength(0); i++)
                publicKeyToken += string.Format("{0:x2}", bytes[i]);

            return publicKeyToken;
        }

    }
}
