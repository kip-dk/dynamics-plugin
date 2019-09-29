namespace Kipon.Xrm.Extensions.Strings
{
    using System.Linq;
    public static class StringMethods
    {
        public static string FirstToUpper(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.First().ToString().ToUpper() + value.Substring(1).ToLower();
        }
    }
}
