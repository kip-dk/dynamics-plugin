using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository
{
    internal interface IEntityShadow
    {
        Microsoft.Xrm.Sdk.Entity Get(string logicalName, Guid id);
        void Update(Microsoft.Xrm.Sdk.Entity entity);
        void Delete(string logicalName, Guid id);
        void Create(Microsoft.Xrm.Sdk.Entity entity);

    }
}
