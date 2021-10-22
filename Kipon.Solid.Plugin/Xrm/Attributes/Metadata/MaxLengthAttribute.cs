namespace Kipon.Xrm.Attributes.Metadata
{
    public class  MaxLengthAttribute : System.Attribute
    {
        public MaxLengthAttribute(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }
    }
}
