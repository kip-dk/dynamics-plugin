using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Model
{
    public interface IPhonenumberChanged : Kipon.Xrm.ITarget
    {
        string[] Fields { get; }
        Microsoft.Xrm.Sdk.AttributeCollection Attributes { get; }
    }
}
