using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IPublishereService))]
    public class PublishereService : ServiceAPI.IPublishereService
    {
        private readonly Entities.IUnitOfWork uow;
        private string _componentString;

        [ImportingConstructor]
        public PublishereService(Entities.IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public string ComponentPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(_componentString))
                {
                    var sol = Kipon.Xrm.Tools.Models.Config.Instance?.Solution;
                    if (string.IsNullOrEmpty(sol))
                    {
                        throw new Exception("No solution found in configuration setup");
                    }

                    var pub = (from p in uow.Publishers.GetQuery()
                               join s in uow.Solutions.GetQuery() on p.PublisherId equals s.PublisherId.Id
                               where s.UniqueName == sol
                               select new
                               {
                                   Id = p.PublisherId.Value,
                                   Prefix = p.CustomizationPrefix
                               }).SingleOrDefault();

                    if (pub == null)
                    {
                        throw new Exception($"Could not find solution with unique name: { sol }");
                    }

                    uow.Detach(Entities.Publisher.EntityLogicalName, pub.Id);
                    this._componentString = pub.Prefix;
                }
                return _componentString;
            }
        }
    }
}
