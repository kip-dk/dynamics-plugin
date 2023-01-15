using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    internal class TracingService : Microsoft.Xrm.Sdk.ITracingService
    {
        public void Trace(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }

        private static Microsoft.Xrm.Sdk.ITracingService _instance;

        public static Microsoft.Xrm.Sdk.ITracingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TracingService();
                }
                return _instance;
            }
        }
    }
}
