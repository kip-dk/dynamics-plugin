
namespace Kipon.Xrm
{
    public interface ActionTarget<T> where T: Microsoft.Xrm.Sdk.Entity
    {
        Microsoft.Xrm.Sdk.EntityReference Target { get; }
    }
}
