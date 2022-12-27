using Kipon.Xrm.Tools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.INugetService))]
    public class NugetService : ServiceAPI.INugetService
    {
        private Models.Config config;
        private Models.NugetSpec spec;

        public NugetService()
        {
            this.config = Models.Config.Instance;
        }

        public Models.NugetSpec GetSpec()
        {
            if (spec != null)
            {
                return spec;
            }

            Models.Config.ThrowIFMissing();

            this.spec = new Kipon.Xrm.Tools.Models.NugetSpec(config.Plugin.Spec);
            return spec;
        }

        public DLLCode[] GetLibNet64()
        {
            var nuget = this.GetSpec();

            using (ZipArchive zip = ZipFile.OpenRead(this.config.Plugin.Package.Replace("$version",nuget.Metadata.Version)))
            {
                var result = new List<Models.DLLCode>();

                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.StartsWith("lib/net462/") && entry.Name != "Kipon.Xrm.dll")
                    {
                        var next = new DLLCode
                        {
                            Name = entry.Name
                        };
                        using (var ent = entry.Open())
                        {
                            using (var mem = new System.IO.MemoryStream())
                            {
                                ent.CopyTo(mem);
                                next.Code = mem.ToArray();
                            }
                            result.Add(next);
                        }
                    }
                }
                return result.ToArray();
            }
        }
    }
}
