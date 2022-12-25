using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Extensions.Strings;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.ConfigTest", typeof(ICmd))]
    public class ConfigTest : ICmd
    {
        public Task ExecuteAsync(string[] args)
        {
            var file = "config".CommandlineValue();

            if (string.IsNullOrEmpty(file))
            {
                Console.WriteLine($"Please provide name of config file as command line parameter on form /config:filename");
                return Task.CompletedTask;
            }

            Console.WriteLine(Kipon.Xrm.Tools.Models.Config.Instance.ConnectionString);
            return Task.CompletedTask;
        }
    }
}
