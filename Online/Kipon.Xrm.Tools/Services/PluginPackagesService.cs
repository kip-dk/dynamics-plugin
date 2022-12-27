using Kipon.Xrm.Tools.Entities;
using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IPluginPackagesService))]
    public class PluginPackagesService : ServiceAPI.IPluginPackagesService
    {
        private readonly Entities.IUnitOfWork uow;
        private readonly IFileService fileService;

        [ImportingConstructor]
        public PluginPackagesService(Entities.IUnitOfWork uow, ServiceAPI.IFileService fileService)
        {
            this.uow = uow;
            this.fileService = fileService;
        }

        public PluginPackage GetPluginPackage(string name)
        {
            return (from p in this.uow.PluginPackages.GetQuery()
                    where p.UniqueName == name
                    select p).SingleOrDefault();
        }

        public Guid Create(string display, string name, string version, byte[] nugetpackage)
        {
            if (string.IsNullOrEmpty(display))
            {
                display = name;
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"You must provide a unique name for the plugin package to be created");
            }

            var clean = new Entities.PluginPackage
            {
                PluginPackageId = Guid.NewGuid(),
                UniqueName = name,
                Content = System.Convert.ToBase64String(nugetpackage),
                Version = version,
                Name = display,
            };
            uow.Create(clean);

            return clean.PluginPackageId.Value;
        }

        public void Update(Guid pluginPackageId, string version, byte[] nugetpackage)
        {
            var clean = new Entities.PluginPackage
            {
                PluginPackageId = pluginPackageId,
                Content = System.Convert.ToBase64String(nugetpackage),
                Version = version,
            };
            uow.Update(clean);
        }

        public void Delete(Guid packageId)
        {
            uow.Delete(new Entities.PluginPackage { PluginPackageId = packageId });
        }
    }
}
