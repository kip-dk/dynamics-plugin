namespace Kipon.Xrm
{
    using System;

    public interface IPluginContext
    {
        string UnsecureConfig { get; }
        string SecureConfig { get; }
        Guid UserId { get; }
        CrmEventType EventType { get; }
        bool AttributeChanged(params string[] names);
    }
}
