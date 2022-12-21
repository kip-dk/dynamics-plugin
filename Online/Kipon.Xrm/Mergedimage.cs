namespace Kipon.Xrm
{
    // Declarative interface to request a merged image
    // A merged image takes the target of a plugin, and combine it with the field available in the target.
    // A merged image should
    // In pre stage, a merged image is keept in sync. with the target, so any change to the target will reflect in the merged image, AND vise-versa.
    public interface Mergedimage<T> where T: Microsoft.Xrm.Sdk.Entity
    {
        System.Guid Id { get; }
        string LogicalName { get; }
    }
}
