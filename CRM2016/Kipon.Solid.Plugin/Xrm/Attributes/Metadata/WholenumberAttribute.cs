namespace Kipon.Xrm.Attributes.Metadata
{
    public class WholenumberAttribute : System.Attribute
    {
        public WholenumberAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        public int Min { get; private set; }
        public int Max { get; private set; }
    }
}
