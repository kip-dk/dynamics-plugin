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
            Console.WriteLine($"Kipon.Xrm.Cmd {Kipon.Xrm.Tools.Version.No}");

            if (args != null && args.Where(r => r == "debug").Any())
            {
                Console.WriteLine("Attach debugger to cmd.exe now, and press enter");
                Console.ReadLine();
            }

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
                ICmd cmd = null;
                try
                {
                    cmd = container.GetExportedValue<ICmd>(args[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    var inner = ex.InnerException;

                    while (inner != null)
                    {
                        Console.WriteLine($"  Inner: {inner.Message}");
                        inner = inner.InnerException;
                    }
                    Console.Write("Pres [Enter] to continue.");
                    Console.ReadLine();
                    return;
                }

                await cmd.ExecuteAsync(args.Skip(1).ToArray());
            }
        }
    }
}
