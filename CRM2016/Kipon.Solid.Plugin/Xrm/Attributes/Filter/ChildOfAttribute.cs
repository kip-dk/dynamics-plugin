
namespace Kipon.Xrm.Attributes.Filter
{
    public class ChildOfAttribute : IfAttribute
    {
        public string[] Message { get; private set; }
        public string LogicalName { get; set; }
        public string ReferenceFieldName { get; private set; }
        public ChildOfAttribute(params string[] messagesLogicalNameAndReferenceField) : base(typeof(Implementations.NotNullFilter))
        {
            this.Message = messagesLogicalNameAndReferenceField;

            if (messagesLogicalNameAndReferenceField.Length > 2)
            {

            }
        }
    }
}
