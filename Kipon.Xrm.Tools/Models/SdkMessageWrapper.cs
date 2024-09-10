using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Models
{
    public class SdkMessageWrapper
    {
        public string Name { get; set; }
        public int Code { get; set; }

        public Field[] InputFields { get; set; }
        public Field[] OutputFields { get; set; }

        public class Field
        {
            public string Name { get; set; }
            public string CLRFormatter { get; set; }
            public bool? IsOptional { get; set; }

            public override int GetHashCode()
            {
                return $"{this.Name}:{this.CLRFormatter}:{this.IsOptional}".GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is Field f)
                {
                    return this.Name == f.Name && this.CLRFormatter == f.CLRFormatter && this.IsOptional == f.IsOptional;
                }
                return false;
            }
        }
    }
}
