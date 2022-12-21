namespace Kipon.Xrm
{
    /***
     * Implement this interface to indicate that clear an entity reference can be parsed as an empty guid from the client
     */
    public interface IReplaceEntityReferenceEmptyGuidWithNull
    {
        /***
         *  Return the attributes that can be parsed as empty guid, and safely be replace with null, return null if any reference in the payload support the behavior
         */
        string[] ForAttributes { get; }
    }
}
