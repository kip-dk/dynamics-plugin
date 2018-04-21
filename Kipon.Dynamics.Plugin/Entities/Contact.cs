using System.Text;
namespace Kipon.Dynamics.Plugin.Entities
{
    public partial class Contact
    {
        public string ShortFullname
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FullName))
                {
                    var spl = this.FullName.Split(' ');
                    if (spl.Length <= 2) return this.FullName;
                    var sb = new StringBuilder();
                    for (var i = 0;i < spl.Length;i++)
                    {
                        if (i == 0) sb.Append(spl[i]);
                        else if (i == (spl.Length - 1)) sb.Append(" " + spl[i]);
                        else sb.Append(" " + spl[i].Substring(0, 1).ToUpper() + ".");
                    }
                    return sb.ToString();
                }
                return this.FullName;
            }
        }
    }
}
