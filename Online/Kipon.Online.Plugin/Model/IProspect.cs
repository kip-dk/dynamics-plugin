using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Model
{
    public interface IProspect
    {
        Guid Id { get; }
        string LogicalName { get; }
        Microsoft.Xrm.Sdk.Money CreditLimit { get; }
    }
}
