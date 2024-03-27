using Kipon.Xrm.Tools.ServiceAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("buildmodel", typeof(ICmd))]
    public class GenerateModel : ICmd
    {
        private readonly IAuthStorageService authStorage;

        [ImportingConstructor]
        public GenerateModel(Kipon.Xrm.Tools.ServiceAPI.IAuthStorageService authStorage)
        {
            this.authStorage = authStorage;
        }

        public Task ExecuteAsync(string[] args)
        {
            if (args.Contains("/debug"))
            {
                Console.WriteLine($"Debug flag is not supported when using the kipon xrm tools wrapper for CrmSvcUtil.exe. Call CrmSvcUtil directly instead.");
                args = args.Where(r => r != "/debug").ToArray();
            }

            var ix = 0;
            foreach (var arg in args.ToArray())
            {
                if (arg.StartsWith("/connectionstring:auth="))
                {
                   var _value = Kipon.Xrm.Tools.XrmOrganization.ConnectionString.ResolveConnectionStringFromStorage(arg.Substring(18));
                    args[ix] = $"/connectionstring:{_value}";
                }
                ix++;
            }

            string argString = string.Empty;

            if (args.Length > 0)
            {
                argString = string.Join(" ", args.Select(r => $"\"{r}\""));
            }

            using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
            {
                pProcess.StartInfo.FileName = @"..\bin\coretools\CrmSvcUtil.exe";
                pProcess.StartInfo.Arguments = argString; //argument
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                pProcess.StartInfo.CreateNoWindow = false; //not diplay a windows
                pProcess.Start();

                string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                pProcess.WaitForExit();

                Console.WriteLine(output);
            }

            return Task.CompletedTask;
        }
    }
}
