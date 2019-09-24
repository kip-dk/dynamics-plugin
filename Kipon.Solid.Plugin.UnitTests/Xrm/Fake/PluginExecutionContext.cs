using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Fake
{
    public class PluginExecutionContext : Microsoft.Xrm.Sdk.IPluginExecutionContext
    {
        public static readonly Guid USERID = new Guid("B6D6F207-47B9-4178-B7AC-5166A1BECDA9");
        public static readonly Guid BUSINESSUNITID = new Guid("4FF76744-5E7F-4710-AEEF-295B425148A0");
        public static readonly Guid ORGANIZATIONID = new Guid("8DE29C74-0C30-4239-AC0B-91646CF89A64");
        public static readonly Guid CORRLID = new Guid("2B2D4925-F01C-4A69-B447-3ED859B2914D");

        private ParameterCollection inputParam = new ParameterCollection();
        private ParameterCollection outputParam = new ParameterCollection();
        private ParameterCollection sharedParam = new ParameterCollection();

        private EntityImageCollection preImages = new EntityImageCollection();
        private EntityImageCollection posImages = new EntityImageCollection();

        private int stage;
        private int dept;
        private string message;
        private string primaryEntityName;
        private Guid primaryEntityId;
        private bool async;

        public PluginExecutionContext(int stage, int dept, string message, string primaryEntityName, Guid primaryEntityId, bool async)
        {
            this.stage = stage;
            this.dept = dept;
            this.message = message;
            this.primaryEntityName = primaryEntityName;
            this.primaryEntityId = primaryEntityId;
            this.async = async;
        }

        public static PluginExecutionContext ForMethodWithTarget(System.Reflection.MethodInfo method, Microsoft.Xrm.Sdk.Entity target)
        {
            var stage = method.Name.ToStage();
            var message = method.Name.ToMessage();

            var result = new PluginExecutionContext(stage, 0, message, target.LogicalName, target.Id, method.Name.Contains("Async"));
            result.InputParameters["Target"] = target;
            return result;
        }

        public int Stage => this.stage;

        public IPluginExecutionContext ParentContext => null;

        public int Mode => async ? 1 : 0;

        public int IsolationMode => 0;

        public int Depth => this.dept;

        public string MessageName => this.message;

        public string PrimaryEntityName => this.primaryEntityName;
        public Guid PrimaryEntityId => this.primaryEntityId;

        public Guid? RequestId => Guid.NewGuid();

        public string SecondaryEntityName => null;

        public ParameterCollection InputParameters => inputParam;

        public ParameterCollection OutputParameters => outputParam;

        public ParameterCollection SharedVariables => sharedParam;

        public Guid UserId => USERID;

        public Guid InitiatingUserId => USERID;

        public Guid BusinessUnitId => BUSINESSUNITID;

        public Guid OrganizationId => ORGANIZATIONID;

        public string OrganizationName => "Kipon Fake.";

        public EntityImageCollection PreEntityImages => preImages;

        public EntityImageCollection PostEntityImages => posImages;

        public EntityReference OwningExtension => null;

        public Guid CorrelationId => CORRLID;

        public bool IsExecutingOffline => false;

        public bool IsOfflinePlayback => false;

        public bool IsInTransaction => this.Stage > 10;

        public Guid OperationId => Guid.NewGuid();

        public DateTime OperationCreatedOn => System.DateTime.Now;
    }

    public static class PluginExecutionContextLocalExtensions
    {
        public static int ToStage(this string value)
        {
            if (value.Contains("Validate")) return 10;
            if (value.Contains("Pre")) return 20;
            if (value.Contains("Post")) return 40;

            throw new Exception($"Unable to resolve stage from value {value}");
        }

        public static string ToMessage(this string value)
        {
            if (value.Contains("Create")) return "Create";
            if (value.Contains("Update")) return "Update";
            if (value.Contains("Delete")) return "Delete";
            throw new Exception($"Unable to resolve message from value {value}");
        }
    }
}
