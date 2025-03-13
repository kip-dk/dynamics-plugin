namespace Kipon.Xrm.Attributes
{
    using System;
    /// <summary>
    /// Use this method in plugins to state that the method should be called as a step.
    /// The recommended approach is using naming conventions, but if for some reason this cannot be used, add the attribute to the method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StepAttribute : Attribute
    {

        public StepAttribute(StageEnum stage, MessageEnum message, string primaryEntityName)
        {
            this.Stage = (int)stage;
            this.Message = message.ToString();
            this.PrimaryEntityName = primaryEntityName;
        }

        public int Stage { get; private set; }
        public string Message { get; private set; }
        public string PrimaryEntityName { get; set; }

        public bool IsAsync
        {
            get
            {
                return this.Stage == (int)StageEnum.PostAsync;
            }
        }

        public enum StageEnum
        {
            Validate = 10,
            Pre = 20,
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
            RetrieveMultiple,
            Retrieve,
            AddMember,
            AddListMembers,
            RemoveListMembers,
            RemoveMember,
            Merge
        }
    }
}
