using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;

namespace Kipon.Solid.Plugin.Workflow
{
    public sealed class HelloWorkflowWorld : Kipon.Xrm.BaseCodeActivity
    {

        #region imports
        [Kipon.Xrm.Attributes.Admin]
        [Kipon.Xrm.Attributes.Import]
        public Kipon.Solid.Plugin.Entities.IUnitOfWork uow { get; set; }
        #endregion

        #region input
        [RequiredArgument]
        [ReferenceTarget(Entities.Account.EntityLogicalName)]
        [Input("AccountId")]
        public InArgument<EntityReference> AccountId { get; set; }

        #endregion

        #region output
        [Output("Children")]
        public OutArgument<int> Count { get; set; }


        [Output("WholeNumber")]
        public OutArgument<int> WholeNumber { get; set; }


        [Output("DecimalNumber")]
        public OutArgument<decimal> DecimalNumber { get; set; }

        [Output("DoubleNumber")]
        public OutArgument<double> DoubleNumber { get; set; }

        [Output("MoneyNumber")]
        public OutArgument<Microsoft.Xrm.Sdk.Money> MoneyNumber { get; set; }

        [Output("OptionSetNumber")]
        [AttributeTarget(Entities.Account.EntityLogicalName, "accountcategorycode")]
        public OutArgument<Microsoft.Xrm.Sdk.OptionSetValue> OptionSetValue { get; set; }

        [Output("TextValue")]
        public OutArgument<string> StringValue { get; set; }

        [Output("BooleanValue")]
        public OutArgument<bool> BooleanValue { get; set; }

        [Output("DateTimeValue")]
        public OutArgument<System.DateTime> DateTimeValue { get; set; }

        #endregion

        protected override void Run(CodeActivityContext executionContext)
        {
            var count = (from a in uow.Accounts.GetQuery()
                         where a.ParentAccountId.Id == this.AccountId.Get(executionContext).Id
                         select a.AccountId.Value).ToArray().Count();

            this.Count.Set(executionContext, count);
            this.WholeNumber.Set(executionContext, 10);
            this.DecimalNumber.Set(executionContext, 10M);
            this.DoubleNumber.Set(executionContext, 10d);
            this.BooleanValue.Set(executionContext, true);
            this.OptionSetValue.Set(executionContext, new Microsoft.Xrm.Sdk.OptionSetValue(1));
            this.DateTimeValue.Set(executionContext, System.DateTime.UtcNow);
            this.MoneyNumber.Set(executionContext, new Money(0M));
            this.StringValue.Set(executionContext, "text value");
        }
    }
}
