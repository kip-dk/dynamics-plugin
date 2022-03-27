
namespace Kipon.Xrm.Attributes.Filter
{
    public class NotNullAttribute : IfAttribute
    {
        public string[] Attributes { get; private set; }
        public NotNullAttribute(params string[] attributes) : base(typeof(Implementations.NotNullFilter))
        {
            this.Attributes = attributes;
        }
    }
}
