namespace Kipon.Xrm
{

    /// <summary>
    /// Declarativ inteface to represent the target of a plugin
    /// Any extension can have getters and setters, and if changed in a pre process, any change will be send back
    /// to the server together with the initial state of the target
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface Target<T> where T: Microsoft.Xrm.Sdk.Entity
    {
    }
}
