using Kipon.Xrm.Tools.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.ISolutionComponentsService))]
    public class SolutionComponentsService : ServiceAPI.ISolutionComponentsService
    {
        private readonly Entities.IUnitOfWork uow;

        [ImportingConstructor]
        public SolutionComponentsService(Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }
        public SolutionComponent[] GetComponentsForSolution(Guid solutionId)
        {
            return (from c in this.uow.SolutionComponents.GetQuery()
                    where c.SolutionId.Id == solutionId
                    select c).ToArray();
        }
    }
}
