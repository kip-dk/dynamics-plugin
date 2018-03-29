using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Dynamics.Plugin.ServiceAPI
{
    public interface IContactService
    {
        void UpdateKing(Entities.Contact target, string jobtitle = null);
    }
}
