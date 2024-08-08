
namespace Kipon.Xrm
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Tracer :IDisposable
    {
        private static Dictionary<System.Threading.Thread, Microsoft.Xrm.Sdk.ITracingService> SERVICES = new Dictionary<System.Threading.Thread, Microsoft.Xrm.Sdk.ITracingService>();

        private bool isRoot = true;
        public Tracer(Microsoft.Xrm.Sdk.ITracingService trace)
        {
            if (!SERVICES.ContainsKey(System.Threading.Thread.CurrentThread))
            {
                SERVICES[System.Threading.Thread.CurrentThread] = trace;
                this.isRoot = true;
            } else
            {
                var me = SERVICES[System.Threading.Thread.CurrentThread];

                if (trace != me)
                {
                    trace.Trace($"TRACE: Two different instance of trace found on same thread, that is unexpected");
                    me.Trace($"ME: Two different instance of trace found on same thread, that is unexpected");
                } else
                {
                    trace.Trace($"Nested Tracer found. You do not need to create local instance of Tracer. The kipon base plugin has the only needed instance for the plugin flow.");
                }
                this.isRoot = false;
            }
        }


        public static void Trace(string message)
        {
            var ts = traceService;
            if (ts != null)
            {
                ts.Trace(message);
            }
        }

        private static Microsoft.Xrm.Sdk.ITracingService traceService
        {
            get
            {
                var xid = System.Threading.Thread.CurrentThread;
                if (SERVICES.TryGetValue(xid, out Microsoft.Xrm.Sdk.ITracingService ts))
                {
                    return ts;
                }
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                if (this.isRoot)
                {
                    var xid = System.Threading.Thread.CurrentThread;
                    if (SERVICES.ContainsKey(xid))
                    {
                        SERVICES.Remove(xid);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
