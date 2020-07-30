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
        private static readonly string END = "/XRMServices/2011/Organization.svc".ToLower();

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

                        if (_value == null)
                        {
                            var url = (from p in parameters where p.ToLower().StartsWith("/url:") select p).SingleOrDefault()?.Substring(5);
                            var usr = (from p in parameters where p.ToLower().StartsWith("/username:") select p).SingleOrDefault()?.Substring(10);
                            var pwd = (from p in parameters where p.ToLower().StartsWith("/password:") select p).SingleOrDefault()?.Substring(10);
                            var dom = (from p in parameters where p.ToLower().StartsWith("/domain:") select p).SingleOrDefault()?.Substring(8);

                            if (url != null && usr != null && pwd != null && dom != null)
                            {
                                if (url.ToLower().EndsWith(END)) 
                                {
                                    url = url.Substring(0, url.Length - END.Length);
                                }
                                _value = $"Url={url};UserName={usr};password={pwd};AuthType=AD;Domain={dom}";
                            }

                            if (_value == null && url != null && usr == null && pwd == null && dom == null)
                            {
                                _value = $"Url={url};AuthType=AD;";
                            }
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
