using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.B64DecodeTest", typeof(ICmd))]
    public class B64DecodeTest : ICmd
    {
        public Task ExecuteAsync(string[] args)
        {
            var content = System.IO.File.ReadAllText(@"C:\Temp\b64.txt");

            var decode = System.Convert.FromBase64String(content);

            System.IO.File.WriteAllBytes(@"C:\Temp\b64-result.txt", decode);

            return Task.CompletedTask;
        }
    }
}
