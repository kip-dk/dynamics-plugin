using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration.Entities
{
    public partial class SolutionComponent
    {
        public enum ComponentTypeEnum
        {
            Attachment = 35,
            Attribute = 2,
            AttributeLookupValue = 5,
            AttributeMap = 47,
            AttributePicklistValue = 4,
            ConnectionRole = 63,
            ContractTemplate = 37,
            ConvertRule = 154,
            ConvertRuleItem = 155,
            CustomControl = 66,
            CustomControlDefaultConfig = 68,
            DisplayString = 22,
            DisplayStringMap = 23,
            DuplicateRule = 44,
            DuplicateRuleCondition = 45,
            EmailTemplate = 36,
            Entity = 1,
            EntityKey = 14,
            EntityMap = 46,
            EntityRelationship = 10,
            EntityRelationshipRelationships = 12,
            EntityRelationshipRole = 11,
            FieldPermission = 71,
            FieldSecurityProfile = 70,
            Form = 24,
            HierarchyRule = 65,
            KBArticleTemplate = 38,
            LocalizedLabel = 7,
            MailMergeTemplate = 39,
            ManagedProperty = 13,
            MobileOfflineProfile = 161,
            MobileOfflineProfileItem = 162,
            OptionSet = 9,
            Organization = 25,
            PluginAssembly = 91,
            PluginType = 90,
            Relationship = 3,
            RelationshipExtraCondition = 8,
            Report = 31,
            ReportCategory = 33,
            ReportEntity = 32,
            ReportVisibility = 34,
            RibbonCommand = 48,
            RibbonContextGroup = 49,
            RibbonCustomization = 50,
            RibbonDiff = 55,
            RibbonRule = 52,
            RibbonTabToCommandMap = 53,
            Role = 20,
            RolePrivilege = 21,
            RoutingRule = 150,
            RoutingRuleItem = 151,
            SavedQuery = 26,
            SavedQueryVisualization = 59,
            SDKMessageProcessingStep = 92,
            SDKMessageProcessingStepImage = 93,
            ServiceEndpoint = 95,
            SimilarityRule = 165,
            SiteMap = 62,
            SLA = 152,
            SLAItem = 153,
            SystemForm = 60,
            ViewAttribute = 6,
            WebResource = 61,
            Workflow = 29
        }

        private ComponentKey _key = null;
        public ComponentKey Key
        {
            get
            {
                if (_key == null)
                {
                    _key = new ComponentKey { ComponentType = this.ComponentType.Value, ObjectId = this.ObjectId.Value };
                }
                return _key;
            }
        }

        public class ComponentKey
        {
            public int ComponentType { get; set; }
            public Guid ObjectId { get; set; }

            public override int GetHashCode()
            {
                return (ComponentType.ToString() + "|" + ObjectId.ToString()).GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as ComponentKey;
                if (other != null)
                {
                    return this.ComponentType == other.ComponentType && this.ObjectId == other.ObjectId;
                }
                return false;
            }
        }
    }
}
