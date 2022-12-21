using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.Relationship
{
    public class AssociatePlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreAssociate(Kipon.Xrm.Actions.IAssociateRequest request)
        {
            var name = request.Relationship.SchemaName;
            var target = request.Target;
            var others = request.RelatedEntities;
        }

        [Kipon.Xrm.Attributes.Relationship("systemuserroles_association")]
        public void OnPreDisassociate(Kipon.Xrm.Actions.IAssociateRequest request)
        {
            var name = request.Relationship.SchemaName;
            var target = request.Target;
            var others = request.RelatedEntities;
        }
    }
}
