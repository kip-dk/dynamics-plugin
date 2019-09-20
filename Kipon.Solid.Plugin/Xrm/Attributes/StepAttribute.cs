using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Attributes
{
    public class StepAttribute : Attribute
    {

        public StepAttribute(StageEnum stage, MessageEnum message, string primaryEntityName)
        {
            this.Stage = stage;
            this.Message = message;
            this.PrimaryEntityName = primaryEntityName;
        }

        public StageEnum Stage { get; private set; }
        public MessageEnum Message { get; private set; }
        public string PrimaryEntityName { get; set; }

        public bool IsAsync
        {
            get
            {
                return this.Stage == StageEnum.PostAsync;
            }
        }

        public enum StageEnum
        {
            Validate = 10,
            Pre = 10,
            Post = 40,
            PostAsync = 41
        }

        public enum MessageEnum
        {
            Create,
            Update,
            Delete,
            Associate,
            Disassociate,
            SetState,
            SetStateDynamicEntity,
            RetrieveMultiple,
            Retrieve,
            Other,
            AddMember,
            AddListMembers,
            RemoveMember,
            Merge
        }
    }
}
