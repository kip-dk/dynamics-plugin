using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration.Model
{
    public class Step
    {
        public Type Class { get; set; }
        public Solution Solution { get; set; }
        public CrmEventType EventType { get; set; }
        public string PrimaryEntity { get; set; }
        public string SecondaryEntity { get; set; }
        public string[] FilteringAttributes { get; set; }
        public int ExecutionOrder { get; set; }
        public StageEnum Stage { get; set; }
        public bool Preimage { get; set; }
        public string[] PreimageAttributes { get; set; }
        public bool Offline { get; set; }

        public int Async
        {
            get
            {
                return (this.Stage == StageEnum.PostOperationAsyncWithDelete || this.Stage == StageEnum.PostOperationAsyncWithoutDelete) ? 1 : 0;
            }
        }

        public int StageValue
        {
            get
            {
                switch (this.Stage)
                {
                    case StageEnum.PostOperation: return 40;
                    case StageEnum.PostOperationAsyncWithDelete: return 40;
                    case StageEnum.PostOperationAsyncWithoutDelete: return 40;
                    case StageEnum.PreValidate: return 10;
                    case StageEnum.PreOperation: return 20;
                    default: throw new ArgumentException("Stage " + Stage.ToString() + " has not been mapeed");
                }
            }
        }

        public string MessagePropertyName
        {
            get
            {
                switch (EventType)
                {
                    case CrmEventType.Associate:
                    case CrmEventType.Disassociate:
                    case CrmEventType.SetState:
                    case CrmEventType.SetStateDynamicEntity:
                    case CrmEventType.Close:
                        return "EntityMoniker";
                    case CrmEventType.Delete:
                    case CrmEventType.Update:
                        return "Target";
                    case CrmEventType.Create:
                        return "Id";
                    default: throw new ArgumentException("MessagePropertyName has not been maped for " + EventType);
                }
            }
        }

        public bool DeleteAfterSuccess
        {
            get
            {
                return this.Stage == StageEnum.PostOperationAsyncWithDelete;
            }
        }

        public string Name
        {
            get
            {
                return Class.FullName + ": " + EventType.ToString() + " of " + PrimaryEntity;
            }
        }

        public string UniqueName
        {
            get
            {
                return this.Name + "/" + (int)this.StageValue + "/" + this.ExecutionOrder + "/" + this.Async.ToString();
            }
        }
    }
}
