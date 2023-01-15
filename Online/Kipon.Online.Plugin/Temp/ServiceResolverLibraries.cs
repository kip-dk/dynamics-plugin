using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Temp
{
    public class ServiceResolverLibraries : Kipon.Xrm.ServiceAPI.IServiceResolverLibraries
    {
        public string[] Fullnames => new string[] 
        {
            "Kipon.Online.Plugin.Services, Version=1.0.0.3, Culture=neutral, PublicKeyToken=null"
        };
    }
}
