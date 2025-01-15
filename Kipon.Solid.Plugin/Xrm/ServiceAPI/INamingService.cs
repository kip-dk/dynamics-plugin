
namespace Kipon.Xrm.ServiceAPI
{
    using System;
    public interface INamingService
    {
        /// <summary>
        /// If refid is not null, but name is null, the service will try to populate the name from the SDK, and return the result. 
        /// If found the name will also be populated in refid.Name
        /// If Name is already populated on the entityreference, that name will be return.
        /// </summary>
        /// <param name="refid">Reference to lookup name for</param>
        /// <returns>null or name of the eference instance</returns>
        string NameOf(Microsoft.Xrm.Sdk.EntityReference refid);
        string Concat(params Microsoft.Xrm.Sdk.EntityReference[] refs);
        string Concat(string sep, params Microsoft.Xrm.Sdk.EntityReference[] refs);
        Microsoft.Xrm.Sdk.EntityReference[] NamesOf(string entityLogicalName, params Guid[] ids);

        /// <summary>
        /// Returns the primary attributeid for the entity
        /// </summary>
        /// <param name="entitylogicalname"></param>
        /// <returns></returns>
        string PrimaryAttributeId(string entitylogicalname);

        /// <summary>
        /// returns the attribute name that carries the name of the entity.
        /// </summary>
        /// <param name="entitylogicalname"></param>
        /// <returns></returns>
        string PrimaryAttributeName(string entitylogicalname);
    }
}
