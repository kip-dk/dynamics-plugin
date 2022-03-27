
namespace Kipon.Xrm.ServiceAPI
{
    public interface IEntityCache
    {
        System.Collections.Generic.IEnumerable<Microsoft.Xrm.Sdk.Entity> GetAttachedEntities();
        bool Detach(Microsoft.Xrm.Sdk.Entity entity);
        void Attach(Microsoft.Xrm.Sdk.Entity entity);
    }
}
