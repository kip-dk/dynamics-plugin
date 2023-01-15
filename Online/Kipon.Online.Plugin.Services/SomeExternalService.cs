using Kipon.Online.Plugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Services
{
    public class SomeExternalService : Kipon.Online.Plugin.ServiceAPI.ISomeExternalService
    {
        public SomeExternalService()
        {
        }

        public string GetNameOf(string logicalName, Guid id)
        {
            return $"{ logicalName } { id }:{ this.GetType().Assembly.FullName }, Version: 18";
        }
    }
}
