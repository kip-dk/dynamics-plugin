using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tests
{
    [Export("Tests.AnunboundedactionTest", typeof(ICmd))]
    public class AnunboundedactionTest : ICmd
    {
        private readonly Xrm.Tools.Entities.IUnitOfWork uow;

        [ImportingConstructor]
        public AnunboundedactionTest(Xrm.Tools.Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public Task ExecuteAsync(string[] args)
        {
            return Task.CompletedTask;

        }
    }
}
