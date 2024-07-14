
namespace Kipon.Xrm
{
    using System;
    using System.Collections.Generic;

    public class Tracer :IDisposable
    {
        private static Dictionary<int, Microsoft.Xrm.Sdk.ITracingService> SERVICES = new Dictionary<int, Microsoft.Xrm.Sdk.ITracingService>();

        private bool isRoot = true;
        public Tracer(Microsoft.Xrm.Sdk.ITracingService trace)
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;

            if (!SERVICES.ContainsKey(id))
            {
                SERVICES[System.Threading.Thread.CurrentThread.ManagedThreadId] = trace;
                this.isRoot = true;
            } else
            {
                var me = SERVICES[id];

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
            traceService.Trace(message);
        }

        private static Microsoft.Xrm.Sdk.ITracingService traceService
        {
            get
            {
                return SERVICES[System.Threading.Thread.CurrentThread.ManagedThreadId];
            }
        }

        public void Dispose()
        {
            if (this.isRoot)
            {
                SERVICES.Remove(System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}
