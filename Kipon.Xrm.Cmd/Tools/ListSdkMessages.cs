using Kipon.Xrm.Tools.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export(nameof(ListSdkMessages), typeof(ICmd))]
    public class ListSdkMessages : ICmd
    {
        private readonly IUnitOfWork uow;

        [ImportingConstructor]
        public ListSdkMessages(Kipon.Xrm.Tools.Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public Task ExecuteAsync(string[] args)
        {
            var mess = (from s in uow.SdkMessages.GetQuery()
                        orderby s.Name
                        select new
                        {
                            s.Id,
                            s.Name,
                            s.IsPrivate,
                            s.IsActive
                        }).ToArray();

            foreach (var mes in mess)
            {
                System.IO.File.AppendAllText(@"C:\Temp\Atea2.txt",mes.Name.PadRight(40, ' ') + mes.IsActive + " " + mes.IsPrivate + "\r\n");
            }

            return Task.CompletedTask;
        }
    }
}
