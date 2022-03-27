using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Exceptions
{
    public class EntityNotFoundException : BaseException
    {
        public EntityNotFoundException(string logicalName, Guid id) : base($"Entity {logicalName} with id {id.ToString()} does not exists.")
        {
        }
    }
}
