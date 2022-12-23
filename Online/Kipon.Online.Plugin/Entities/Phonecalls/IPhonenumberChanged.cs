using Kipon.Xrm.Attributes;
using Kipon.Xrm.Extensions.Sdk;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Entities
{
    [TargetFilter(typeof(Model.IPhonenumberChanged), nameof(PhoneCall.PhoneNumber))]
    public partial class PhoneCall : Model.IPhonenumberChanged
    {
        string[] Model.IPhonenumberChanged.Fields => this.TargetFilterAttributesOf(typeof(Model.IPhonenumberChanged));

    }
}
