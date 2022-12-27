using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Tools.Extensions.Strings;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.ListSolutionComponents", typeof(ICmd))]
    public class ListSolutionComponentsTest : ICmd
    {
        private readonly ISolutionService solutionService;
        private readonly ISolutionComponentsService componentService;

        [ImportingConstructor]
        public ListSolutionComponentsTest(Kipon.Xrm.Tools.ServiceAPI.ISolutionService solutionService, Kipon.Xrm.Tools.ServiceAPI.ISolutionComponentsService componentService)
        {
            this.solutionService = solutionService;
            this.componentService = componentService;
        }

        public Task ExecuteAsync(string[] args)
        {
            if (Kipon.Xrm.Tools.Models.Config.Instance == null)
            {
                Console.WriteLine("Please provide config file to be used on command line form /config:filename");
                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(Kipon.Xrm.Tools.Models.Config.Instance.Solution))
            {
                Console.WriteLine("Please provide solution property in config file");
                return Task.CompletedTask;
            }

            var solution = this.solutionService.Get(Kipon.Xrm.Tools.Models.Config.Instance.Solution);
            if (solution == null)
            {
                Console.WriteLine($"No solution found with name: { Kipon.Xrm.Tools.Models.Config.Instance.Solution }");
                return Task.CompletedTask;
            }

            var components = this.componentService.GetComponentsForSolution(solution.SolutionId.Value);
            Console.WriteLine($"Found { components.Length } components in solution { Kipon.Xrm.Tools.Models.Config.Instance.Solution }");

            foreach (var com in components)
            {
                Console.WriteLine($"{ com.ComponentType.Value }  { com.ObjectId.Value }");
            }

            return Task.CompletedTask;
        }
    }
}
