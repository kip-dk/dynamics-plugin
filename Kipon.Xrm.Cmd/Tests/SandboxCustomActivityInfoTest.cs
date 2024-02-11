using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.SandboxCustomActivityInfoTest", typeof(ICmd))]
    public class SandboxCustomActivityInfoTest : ICmd
    {
        public Task ExecuteAsync(string[] args)
        {
            var type = typeof(ActivityTestClass);
            var activity = new Kipon.Xrm.Tools.Models.SandboxCustomActivityInfo(type.Assembly, type);

            var xml = activity.ToXml();

            System.IO.File.WriteAllText(@"C:\Temp\custactivity.xml", xml);

            return Task.CompletedTask;
        }

        public class ActivityTestClass : System.Activities.CodeActivity
        {
            #region input
            [RequiredArgument]
            [ReferenceTarget("account")]
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
            [AttributeTarget("account", "accountcategorycode")]
            public OutArgument<Microsoft.Xrm.Sdk.OptionSetValue> OptionSetValue { get; set; }

            [Output("TextValue")]
            public OutArgument<string> StringValue { get; set; }

            [Output("BooleanValue")]
            public OutArgument<bool> BooleanValue { get; set; }

            [Output("DateTimeValue")]
            public OutArgument<System.DateTime> DateTimeValue { get; set; }

            #endregion


            protected override void Execute(CodeActivityContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
