using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : Model.IProspect, Model.INamed
    {
        public Microsoft.Xrm.Sdk.Money Saldo { get; set; }

        void x()
        {
            this.Attributes.Remove("");

        }
    }
}
