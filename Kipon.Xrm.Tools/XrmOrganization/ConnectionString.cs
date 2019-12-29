using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.XrmOrganization
{
    public class ConnectionString
    {

        private static string _value;

        public static string Value
        {
            get
            {
                if (_value == null)
                {
                    var parameters = System.Environment.GetCommandLineArgs();
                    if (parameters != null && parameters.Length > 0)
                    {
                        var conString = (from p in parameters
                                         where p.ToLower().StartsWith("/connectionstring:")
                                         select p).SingleOrDefault();
                        if (conString != null)
                        {
                            _value = conString.Substring(18);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(_value))
                    {
                        _value = ConfigurationManager.ConnectionStrings["CRM"].ConnectionString;
                    }

                    if (string.IsNullOrWhiteSpace(_value))
                    {
                        throw new Exceptions.ConfigurationException("Unable to find connection string. Ether add /connectionstring:[string] to the command line parameters, or setup a connectionstring named CRM in the related configuration file.");
                    }
                }
                return _value;
            }
        }
    }
}
