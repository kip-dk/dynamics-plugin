
namespace Kipon.Xrm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Tracer :IDisposable
    {
        private static Dictionary<System.Threading.Thread, List<Microsoft.Xrm.Sdk.ITracingService>> SERVICES = new Dictionary<System.Threading.Thread, List<Microsoft.Xrm.Sdk.ITracingService>>();
        private static readonly object locker = new object();
        public Tracer(Microsoft.Xrm.Sdk.ITracingService trace)
        {
            lock (locker)
            {
                if (!SERVICES.ContainsKey(System.Threading.Thread.CurrentThread))
                {
                    SERVICES[System.Threading.Thread.CurrentThread] = new List<Microsoft.Xrm.Sdk.ITracingService>();
                }
                SERVICES[System.Threading.Thread.CurrentThread].Add(trace);
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
                if (SERVICES.TryGetValue(xid, out List<Microsoft.Xrm.Sdk.ITracingService> tss))
                {
                    return tss.LastOrDefault();
                }
                return null;
            }
        }

        public void Dispose()
        {
            try
            {
                lock (locker)
                {
                    var xid = System.Threading.Thread.CurrentThread;
                    if (SERVICES.TryGetValue(xid, out List<Microsoft.Xrm.Sdk.ITracingService> tss))
                    {
                        if (tss.Count > 0)
                        {
                            tss.Remove(tss.Last());
                        }
                    }
                    CleanupEmpty();
                }
            }
            catch (Exception)
            {
            }
        }

        private static void CleanupEmpty()
        {
            try
            {
                foreach (var key in SERVICES.Keys.ToArray())
                {
                    var list = SERVICES[key];
                    if (list.Count == 0)
                    {
                        SERVICES.Remove(key);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
