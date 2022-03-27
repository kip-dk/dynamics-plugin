namespace Kipon.Xrm
{
    public enum CrmEventType
    {
        Create,
        Update,
        Delete,
        Associate,
        Disassociate,
        SetState,
        SetStateDynamicEntity,
        RetrieveMultiple,
        Retrieve,
        Other,
        AddMember,
        AddListMembers,
        RemoveMember,

        QualifyLead,

        CustomPlugin
    }
}
