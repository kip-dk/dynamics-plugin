using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.ListMember
{
    public class ListMemberPlugin : Kipon.Xrm.BasePlugin
    {
        public static readonly Guid LISTID = new Guid("7EE62A35-7142-4E51-B477-D4A389A69FD3");
        public static readonly Guid ENTITYID = new Guid("DC68A195-7000-4854-9846-87A110D8768B");

        public void OnPreRemoveMember(Guid listId, Guid entityId)
        {
            if (listId != LISTID)
            {
                throw new Exception("Expected another lidtId guid for this list");
            }

            if (entityId != ENTITYID)
            {
                throw new Exception("Expected another entityId guid for this list");
            }
        }
    }
}
