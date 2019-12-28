using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("You must specify task name.");
                Console.ReadLine();
                return;
            }

            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Kipon.Xrm.Cmd.Program).Assembly));
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Kipon.Xrm.Tools.CodeCopy.CopyXrmCode).Assembly));

            using (var container = new CompositionContainer(aggregateCatalog))
            {
                var cmd = container.GetExportedValue<ICmd>(args[0]);
                await cmd.ExecuteAsync(args.Skip(1).ToArray());
            }
        }
    }
}
