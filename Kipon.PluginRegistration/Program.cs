using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please provide name of assembly to load");
                return;
            }

            var uow = new Kipon.PluginRegistration.Entities.CrmUnitOfWork();
            var service = new Kipon.PluginRegistration.Services.PluginRegistrationService(uow);
            service.Registre(args[0]);
        }
    }
}
