using Kipon.Xrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Model
{
    public interface IActivitySubject : Kipon.Xrm.IMerged
    {
        string Subject { get; }

        [TargetFilter]
        string EkstraSubject { get; set; }
    }
}
