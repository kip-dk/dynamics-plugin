using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Exceptions
{
    public class EntityExistsException : BaseException
    {
        public EntityExistsException(Microsoft.Xrm.Sdk.Entity entity) : base($"Entity: {entity.LogicalName} + Id: {entity.Id.ToString()} already exists." )
        {
        }
    }
}
