namespace Kipon.Xrm.Extensions.Platform
{
    using System.Reflection;

    public static class PlatformExtensions
    {
        public static Assembly Resolve(this string fullname)
        {
            Assembly res = null;

            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies()) 
            {
                if (ass.FullName == fullname)
                {
                    res = ass;
                }
             }
            return res;
        }
    }
}
