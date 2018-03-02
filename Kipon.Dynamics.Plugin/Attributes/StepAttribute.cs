using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class StepAttribute : Attribute
    {
        public CrmEventType EventType { get; set; }
        public string PrimaryEntity { get; set; }
        public string SecondaryEntity { get; set; }
        public string[] FilteringAttributes { get; set; }
        public int ExecutionOrder { get; set; }
        public StageEnum Stage { get; set; }
        public bool Preimage { get; set; }
        public string[] PreimageAttributes { get; set; }
        public bool Offline { get; set; }

    }
}
