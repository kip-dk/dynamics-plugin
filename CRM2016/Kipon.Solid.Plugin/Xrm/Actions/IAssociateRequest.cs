namespace Kipon.Xrm.Actions
{
    public interface IAssociateRequest
    {
        Microsoft.Xrm.Sdk.Relationship Relationship { get; }
        Microsoft.Xrm.Sdk.EntityReference Target { get; }
        Microsoft.Xrm.Sdk.EntityReferenceCollection RelatedEntities { get;  }
        AssociateType Type { get; }
    }

    public enum AssociateType
    {
        Associate,
        Disassociate
    }
}
