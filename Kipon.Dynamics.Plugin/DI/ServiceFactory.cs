using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.DI
{
    public class ServiceFactory
    {
        private static ServiceFactory sf;

        private static object l1 = new object();

        private Dictionary<Type, Type> exports;
        private static Dictionary<Type, Type> GetTypesWithExportAttribute()
        {
            var assembly = typeof(ServiceFactory).Assembly;
            var exportAttribute = typeof(Kipon.Dynamics.Plugin.DI.Export);
            var result = new Dictionary<Type, Type>();
            foreach (Type type in assembly.GetTypes())
            {
                var ea = type.GetCustomAttributes(exportAttribute, true).SingleOrDefault() as Kipon.Dynamics.Plugin.DI.Export;
                if (ea != null)
                {
                    result[ea.Type] = type;
                }
            }
            return result;
        }


        private ServiceFactory()
        {
            exports = GetTypesWithExportAttribute();
        }

        public static ServiceFactory Instance
        {
            get
            {
                lock(l1)
                {
                    if (ServiceFactory.sf == null)
                    {
                        ServiceFactory.sf = new ServiceFactory();
                    }
                }
                return sf;
            }
        }

        internal void Initialize(Dictionary<Type, object> context, object obj)
        {
            List<object> created = new List<object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                var imp = prop.GetCustomAttributes(typeof(Kipon.Dynamics.Plugin.DI.Import), true).SingleOrDefault();
                if (imp != null)
                {
                    var type = prop.PropertyType;
                    if (context.ContainsKey(type))
                    {
                        prop.SetValue(obj, context[type]);
                    }
                    else
                    {
                        var serviceType = this.exports[type];
                        var service = serviceType.Assembly.CreateInstance(serviceType.FullName);
                        context[type] = service;
                        created.Add(service);
                        prop.SetValue(obj, service);
                    }
                }
            }

            foreach(var no in created)
            {
                Initialize(context, no);
            }
        }

        internal T GetService<T>(Dictionary<Type, object> context) where T: class
        {
            var type = typeof(T);
            if (context.ContainsKey(type))
            {
                return (T)context[type];
            }

            var serviceType = this.exports[type];
            var service = serviceType.Assembly.CreateInstance(serviceType.FullName);
            context[type] = service;
            this.Initialize(context, service);
            return (T)service;
        }
    }
}
