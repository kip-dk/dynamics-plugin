using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Services
{
    public class TracingService : Microsoft.Xrm.Sdk.ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                var mess = string.Format(format, args);
                Console.WriteLine(mess);
            }
            else
            {
                Console.WriteLine(format);
            }
        }
    }
}
