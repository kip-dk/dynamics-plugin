using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.AnunboundedactionTest", typeof(ICmd))]
    public class AnunboundedactionTest : ICmd
    {
        private readonly Xrm.Tools.Entities.IUnitOfWork uow;

        [ImportingConstructor]
        public AnunboundedactionTest(Xrm.Tools.Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public Task ExecuteAsync(string[] args)
        {
            var req = new OrganizationRequest
            {
                RequestName = "kipon_Anunboundedaction"
            };


            req.Parameters.Add(nameof(Kipon.Solid.Plugin.Entities.kipon_AnunboundedactionRequest.Document), "Document");
            req.Parameters.Add(nameof(Kipon.Solid.Plugin.Entities.kipon_AnunboundedactionRequest.Name), "Name");
            req.Parameters.Add(nameof(Kipon.Solid.Plugin.Entities.kipon_AnunboundedactionRequest.Picklist), new { value = 1 });

            Console.WriteLine("Ready");
            this.uow.Execute(req);
            Console.WriteLine("Done");
            return Task.CompletedTask;

        }
    }
}
