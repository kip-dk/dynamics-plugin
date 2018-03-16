using Kipon.Dynamics.Plugin.Attributes;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.DI
{
    public interface IPluginContext
    {
        string UnsecureConfig { get; }
        string SecureConfig { get; }
        Guid UserId { get; }
        CrmEventType EventType { get; }
        Microsoft.Xrm.Sdk.Entity Target { get; }
        Microsoft.Xrm.Sdk.EntityReference TargetReference { get; }
        Microsoft.Xrm.Sdk.EntityReferenceCollection Associations { get; }
        Microsoft.Xrm.Sdk.Entity Preimage { get; }
        Microsoft.Xrm.Sdk.Entity GetFullImage();
        bool AttributeChanged(params string[] names);
        T GetService<T>() where T: class;
    }
}
