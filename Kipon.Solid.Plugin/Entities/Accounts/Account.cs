using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Solid.Plugin.Model;
using Kipon.Xrm;
using Kipon.Xrm.Attributes;
using Microsoft.Xrm.Sdk;
using Kipon.Xrm.Extensions.Sdk;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : Model.IProspect, Model.INamed
    {
        [Microsoft.Xrm.Sdk.AttributeLogicalName("")]
        public Microsoft.Xrm.Sdk.Money Saldo { get; set; }
    }
}
