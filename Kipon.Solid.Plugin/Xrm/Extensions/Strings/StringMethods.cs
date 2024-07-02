namespace Kipon.Xrm.Extensions.Strings
{
    using System.Linq;
    public static class StringMethods
    {
        [System.Diagnostics.DebuggerNonUserCode()]
        public static string FirstToUpper(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.First().ToString().ToUpper() + value.Substring(1).ToLower();
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static string MaxLength(this string value, int maxLen)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.Length <= maxLen)
            {
                return value;
            }
            return value.Substring(0, maxLen);
        }
    }
}
