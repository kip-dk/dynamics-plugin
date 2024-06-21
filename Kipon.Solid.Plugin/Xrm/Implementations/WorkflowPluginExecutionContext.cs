
namespace Kipon.Xrm.Implementations
{
    using Microsoft.Xrm.Sdk;
    using System;

    public class WorkflowPluginExecutionContext : Microsoft.Xrm.Sdk.IPluginExecutionContext
    {
        private Microsoft.Xrm.Sdk.Workflow.IWorkflowContext wfContext;
        private WorkflowPluginExecutionContext parent;


        public WorkflowPluginExecutionContext(Microsoft.Xrm.Sdk.Workflow.IWorkflowContext wfContext)
        {
            this.wfContext = wfContext;
        }

        public int Stage => 0;

        public IPluginExecutionContext ParentContext 
        {
            get
            {
                if (this.wfContext.ParentContext == null) return null;
                if (this.parent == null)
                {
                    this.parent = new WorkflowPluginExecutionContext(wfContext.ParentContext);
                }
                return this.parent;
            }
        }

        public int Mode => this.wfContext.Mode;

        public int IsolationMode => this.wfContext.IsolationMode;

        public int Depth => this.wfContext.Depth;

        public string MessageName => this.wfContext.MessageName;

        public string PrimaryEntityName => this.wfContext.PrimaryEntityName;

        public Guid? RequestId => this.wfContext.RequestId;

        public string SecondaryEntityName => this.wfContext.SecondaryEntityName;

        public ParameterCollection InputParameters => this.wfContext.InputParameters;

        public ParameterCollection OutputParameters => this.wfContext.OutputParameters;

        public ParameterCollection SharedVariables => this.wfContext.SharedVariables;

        public Guid UserId => this.wfContext.UserId;

        public Guid InitiatingUserId => this.wfContext.InitiatingUserId;

        public Guid BusinessUnitId => this.wfContext.BusinessUnitId;

        public Guid OrganizationId => this.wfContext.OrganizationId;

        public string OrganizationName => this.wfContext.OrganizationName;

        public Guid PrimaryEntityId => this.wfContext.PrimaryEntityId;

        public EntityImageCollection PreEntityImages => this.wfContext.PreEntityImages;

        public EntityImageCollection PostEntityImages => this.wfContext.PostEntityImages;

        public EntityReference OwningExtension => this.wfContext.OwningExtension;

        public Guid CorrelationId => this.wfContext.CorrelationId;

        public bool IsExecutingOffline => this.wfContext.IsExecutingOffline;

        public bool IsOfflinePlayback => this.wfContext.IsOfflinePlayback;

        public bool IsInTransaction => this.wfContext.IsInTransaction;

        public Guid OperationId => this.wfContext.OperationId;

        public DateTime OperationCreatedOn => this.wfContext.OperationCreatedOn;
    }
}
