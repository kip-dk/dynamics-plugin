using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Email : Model.IActivitySubject
    {
        [Microsoft.Xrm.Sdk.AttributeLogicalName("kipon_emailextrasubject")]
        public string EkstraSubject { get => this.kipon_emailextrasubject; set => this.kipon_emailextrasubject = value; }
    }
}
