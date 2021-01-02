using Kipon.Xrm.Attributes;
using Kipon.Xrm.Extensions.Sdk;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    [TargetFilter(typeof(Model.IPhonenumberChanged), nameof(Contact.Telephone1), nameof(Contact.Telephone2), nameof(Contact.Telephone3), nameof(Contact.MobilePhone))]
    public partial class Contact : Model.IPhonenumberChanged
    {
        string[] Model.IPhonenumberChanged.Fields => this.TargetFilterAttributesOf(typeof(Model.IPhonenumberChanged));

    }
}
