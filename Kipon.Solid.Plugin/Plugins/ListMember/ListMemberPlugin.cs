using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Kipon.Solid.Plugin.Plugins.ListMember
{
    public class ListMemberPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPostAddMember(Guid ListId, Guid EntityId, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            traceService.Trace($"OnPostAddMember: { ListId }: { EntityId }"); 
        }

        public void OnPostRemoveMember(Guid ListId, Guid EntityId, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            traceService.Trace($"OnPostRemoveMember: {ListId}: {EntityId}");
        }

        public void OnPostAddListMembers(Guid ListId, Guid[] MemberIds, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            traceService.Trace($"OnPostAddListMembers: {ListId}: [{ string.Join(",", MemberIds.Select(r => r.ToString())) }]");
        }

        public void OnPostRemoveListMembers(Guid ListId, Guid EntityId, Microsoft.Xrm.Sdk.ITracingService traceService)
        {
            traceService.Trace($"OnPostRemoveListMembers: {ListId}: {EntityId}");
        }
    }
}
