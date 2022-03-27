using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("xrmcodecopy", typeof(ICmd))]
    public class CodeCopy : ICmd
    {
        public Task ExecuteAsync(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Please provice parameters in this order [sourcepath] [destinationpath] [newnamespace]");
                Console.ReadLine();
                return Task.CompletedTask;
            }

            var source = args[0];
            var dest = args[1];
            var ns = args.Length > 2 ? args[2] : null;

            Kipon.Xrm.Tools.CodeCopy.CopyXrmCode.Copy(source, dest, ns);

            return Task.CompletedTask;
        }
    }
}
