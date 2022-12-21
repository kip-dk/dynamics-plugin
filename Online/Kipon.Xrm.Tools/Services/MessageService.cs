using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IMessageService))]
    public class MessageService : ServiceAPI.IMessageService
    {
        public void Inform(string message)
        {
            Console.WriteLine(message);
        }
    }
}
