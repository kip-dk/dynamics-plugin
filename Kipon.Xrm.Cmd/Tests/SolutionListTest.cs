using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("SolutionListTest", typeof(ICmd))]
    public class SolutionListTest : ICmd
    {
        private readonly ISolutionService solService;

        [ImportingConstructor]
        public SolutionListTest(Kipon.Xrm.Tools.ServiceAPI.ISolutionService solService) 
        {
            this.solService = solService;
        }

        public Task ExecuteAsync(string[] args)
        {
            var sols = this.solService.ListSolutionNames();

            foreach (var sol in sols)
            {
                Console.WriteLine($"{ sol }");
            }
            return Task.CompletedTask;
        }
    }
}
