using System;
using System.Activities.Presentation.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class PhoneCall : Model.IActivitySubject
    {
        [Microsoft.Xrm.Sdk.AttributeLogicalName("kipon_phonecallextrasubject")]
        public string EkstraSubject { get => this.kipon_phonecallextrasubject; set => this.kipon_phonecallextrasubject = value; }

    }
}
