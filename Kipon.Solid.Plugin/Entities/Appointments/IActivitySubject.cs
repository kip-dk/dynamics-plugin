using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Appointment : Model.IActivitySubject
    {
        [Microsoft.Xrm.Sdk.AttributeLogicalName("kipon_appointmentextrasubject")]
        public string EkstraSubject { get => this.kipon_appointmentextrasubject; set => this.kipon_appointmentextrasubject = value; }
    }
}
