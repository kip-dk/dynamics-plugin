using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.ServiceAPI
{
    public interface IAccountService
    {
        void UppercaseName(Entities.Account target);
    }
}
