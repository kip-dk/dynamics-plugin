﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.DI.Reflection
{
    public class MethodArgumentCache
    {
        private static readonly Dictionary<System.Reflection.MethodInfo, MethodArgumentCache> methodTypes = new Dictionary<System.Reflection.MethodInfo, MethodArgumentCache>();


        public static MethodArgumentCache ForMethod(System.Reflection.MethodInfo method)
        {
            if (methodTypes.ContainsKey(method))
            {
                return methodTypes[method];
            }

            var mt = new MethodArgumentCache();
            var parameters = method.GetParameters();
            if (parameters == null || parameters.Length == 0)
            {
                mt.Types = new Type[0];
            }
            else
            {
                mt.Types = new Type[parameters.Length];
                var ix = 0;
                foreach (var p in parameters)
                {
                    mt.Types[ix] = p.ParameterType;
                    ix++;
                }
            }
            methodTypes[method] = mt;
            return mt;
        }

        public Type[] Types { get; set; }


    }
}