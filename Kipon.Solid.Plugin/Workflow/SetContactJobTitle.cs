/*
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;

namespace Kipon.Solid.Plugin.Workflow
{
    public sealed class SetContactJobTitle : Kipon.Xrm.BaseCodeActivity
    {

        #region imports
        [Kipon.Xrm.Attributes.Admin]
        [Kipon.Xrm.Attributes.Import]
        public Kipon.Solid.Plugin.Entities.IUnitOfWork uow { get; set; }
        #endregion

        #region input
        [RequiredArgument]
        [ReferenceTarget(Entities.Contact.EntityLogicalName)]
        [Input("ContactId")]
        public InArgument<EntityReference> ContactId { get; set; }

        [RequiredArgument]
        [Input("JobTitle")]
        public InArgument<string> Title { get; set; }

        #endregion

        protected override void Run(CodeActivityContext executionContext)
        {
            var clean = new Entities.Contact { ContactId = this.ContactId.Get(executionContext).Id };
            clean.JobTitle = this.Title.Get(executionContext);
            this.uow.Update(clean);
        }
    }
}
*/