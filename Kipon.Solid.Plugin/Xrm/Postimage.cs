namespace Kipon.Xrm
{
    // Represent a post image
    // an extension should only expose getters, because any change will not be send back to crm
    public interface Postimage<T> where T: Microsoft.Xrm.Sdk.Entity
    {
    }
}
