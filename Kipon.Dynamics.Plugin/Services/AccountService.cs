using System.Linq;
using Kipon.Dynamics.Plugin.DI;
using Kipon.Dynamics.Plugin.Entities;
using Kipon.Dynamics.Plugin.Extensions.String;
using Microsoft.Xrm.Sdk;

namespace Kipon.Dynamics.Plugin.Services
{
    [Export(typeof(ServiceAPI.IAccountService))]
    public class AccountService : ServiceAPI.IAccountService
    {
        [Import]
        public Entities.IUnitOfWork uow { get; set; }

        [Import]
        public IOrganizationService orgService { get; set; }

        public void UppercaseName(Account target)
        {
            target.Name = target.Name.UpperCaseWords();
        }

        public void Reassign(Entities.Account target)
        {
            if (target.Description != null && target.Description == "to:deploy")
            {
                var user = (from u in uow.Systemusers.GetQuery()
                            where u.DomainName == @"DOM\deploy"
                            select u).SingleOrDefault();

                if (user != null)
                {
                    var request = new Microsoft.Crm.Sdk.Messages.AssignRequest
                    {
                        Target = new EntityReference(Entities.Account.EntityLogicalName, target.AccountId.Value),
                        Assignee = user.ToEntityReference()
                    };
                    this.orgService.Execute(request);
                }
            }
        }
    }
}
