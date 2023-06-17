
namespace Kipon.Xrm.ServiceAPI
{
    public interface IEntityMetadataService
    {
        Microsoft.Xrm.Sdk.Metadata.EntityMetadata ForEntity(string logicalName);
        string[] CanCreateAttributeName(string logicalName);
        string[] CanUpdateAttributeName(string logicalName);
        string PrimaryKey(string logicalName);
        string PrimaryName(string logicalName);
        Microsoft.Xrm.Sdk.Entity CloneForCreate(Microsoft.Xrm.Sdk.Entity source, params string[] ommit);
    }
}
