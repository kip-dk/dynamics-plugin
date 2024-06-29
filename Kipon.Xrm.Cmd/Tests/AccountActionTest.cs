using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("AccountActionTest", typeof(ICmd))]

    public class AccountActionTest : ICmd
    {
        private readonly Kipon.Xrm.Tools.Entities.IUnitOfWork uow;

        [ImportingConstructor]
        public AccountActionTest(Kipon.Xrm.Tools.Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public Task ExecuteAsync(string[] args)
        {
            var req = new OrganizationRequest
            {
                RequestName = "kipon_AccountCountContacts"
            };
            req.Parameters.Add("Target", new Microsoft.Xrm.Sdk.EntityReference("account", new Guid("28E45114-B543-E711-A962-000D3A27D441")));
            req.Parameters.Add("Name", "Kjeld Ingemann Poulsen");

            var res = uow.Execute(req);

            var tot = (int)res.Results["Count"];
            var name = (string)res.Results["FirstVeName"];

            Console.WriteLine($"We execute the action and found  { tot }, { name }");

            return Task.CompletedTask;
        }
    }
}