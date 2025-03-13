namespace Kipon.Xrm
{
    public enum CrmEventType
    {
        Create,
        Update,
        Delete,
        Merge,
        Associate,
        Disassociate,
        SetState,
        SetStateDynamicEntity,
        RetrieveMultiple,
        Retrieve,
        Other,
        AddMember,
        RemoveMember,
        AddListMembers,
        RemoveListMembers,
        QualifyLead,
        GenerateQuoteFromOpportunity,
        CustomPlugin
    }
}
