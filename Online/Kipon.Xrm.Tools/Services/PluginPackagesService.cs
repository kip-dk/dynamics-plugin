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

        public Guid Create(string name, string nugetpackagefilename, byte[] nugetpackage)
        {
            var clean = new Entities.PluginPackage
            {
                PluginPackageId = Guid.NewGuid(),
                UniqueName = name
            };
            uow.Create(clean);

            this.fileService.Upload(nugetpackagefilename, nugetpackage, new Microsoft.Xrm.Sdk.EntityReference(Entities.PluginPackage.EntityLogicalName, clean.PluginPackageId.Value), nameof(clean.FileId).ToLower());
            return clean.PluginPackageId.Value;
        }
    }
}
