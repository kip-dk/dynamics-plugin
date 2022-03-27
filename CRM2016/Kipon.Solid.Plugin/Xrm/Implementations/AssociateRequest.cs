namespace Kipon.Xrm.Implementations
{
    using Microsoft.Xrm.Sdk;

    public class AssociateRequest : Actions.AbstractActionRequest, Actions.IAssociateRequest
    {
        private Actions.AssociateType type;
        public AssociateRequest(Microsoft.Xrm.Sdk.IPluginExecutionContext ctx): base(ctx)
        {
            switch (ctx.MessageName)
            {
                case "Associate": type = Actions.AssociateType.Associate; break;
                case "Disassociate": type = Actions.AssociateType.Disassociate; break;
                default: throw new InvalidPluginExecutionException($"Message {ctx.MessageName} cannot be mapped to an Associate type.");
            }
        }

        public Relationship Relationship => this.ValueOf<Relationship>("Relationship");

        public EntityReference Target => this.ValueOf<Microsoft.Xrm.Sdk.EntityReference>("Target");

        public EntityReferenceCollection RelatedEntities => this.ValueOf<EntityReferenceCollection>("RelatedEntities");

        public Actions.AssociateType Type => type;
    }
}
