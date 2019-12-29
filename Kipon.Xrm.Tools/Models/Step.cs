using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Models
{
    public class Step
    {
        public int Stage { get; set; }
        public string Message { get; set; }
        public string PrimaryEntityLogicalName { get; set; }
        public int ExecutionOrder { get; set; }
        public string[] FilteringAttributes { get; set; }
        public bool IsAsync { get; set; }
        public Image PreImage { get; set; }
        public Image PostImage { get; set; }
    }
}
