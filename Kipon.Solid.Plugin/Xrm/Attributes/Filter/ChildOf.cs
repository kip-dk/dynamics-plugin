
namespace Kipon.Xrm.Attributes.Filter
{
    public class ChildOfAttribute : IfAttribute
    {
        public ChildOfAttribute(string message, string entityLogicalName, string referenceAttributeName = null) : base(typeof(Implementations.ChildOfFilter))
        {
            Message = message;
            EntityLogicalName = entityLogicalName;
            ReferenceAttributeName = referenceAttributeName;
        }

        public string Message { get; }
        public string EntityLogicalName { get; }
        public string ReferenceAttributeName { get; }
    }
}
