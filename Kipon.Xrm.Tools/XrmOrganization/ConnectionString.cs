using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
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
                            _value = ResolveConnectionStringFromStorage(conString.Substring(18));
                        }

                        if (_value == null)
                        {
                            var url = (from p in parameters where p.ToLower().StartsWith("/url:") select p).SingleOrDefault()?.Substring(5);
                            var usr = (from p in parameters where p.ToLower().StartsWith("/username:") select p).SingleOrDefault()?.Substring(10);
                            var pwd = (from p in parameters where p.ToLower().StartsWith("/password:") select p).SingleOrDefault()?.Substring(10);
                            var dom = (from p in parameters where p.ToLower().StartsWith("/domain:") select p).SingleOrDefault()?.Substring(8);
                            var auth = (from p in parameters where p.ToLower().StartsWith("/authtype:") select p).SingleOrDefault()?.Substring(11);

                            if (url != null && usr != null && pwd != null && dom != null)
                            {
                                if (url.ToLower().EndsWith(END)) 
                                {
                                    url = url.Substring(0, url.Length - END.Length);
                                }
                                _value = $"Url={url};UserName={usr};password={pwd};AuthType=AD;Domain={dom}";
                            }

                            if (_value == null && url != null && usr != null && pwd != null && dom == null)
                            {
                                if (auth == null)
                                {
                                    auth = "Office365";
                                }

                                _value = $"Url={url};UserName={usr};password={pwd};AuthType={auth}";
                            }

                            if (_value == null && url != null && usr == null && pwd == null && dom == null)
                            {
                                _value = $"Url={url};AuthType=AD;";
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(_value))
                    {
                        _value = ConfigurationManager.ConnectionStrings["CRM"]?.ConnectionString;

                        _value = ResolveConnectionStringFromStorage(_value);
                    }

                    if (string.IsNullOrWhiteSpace(_value))
                    {
                        throw new Exceptions.ConfigurationException("Unable to find connection string. Ether add /connectionstring:[string] to the command line parameters, or setup a connectionstring named CRM in the related configuration file.");
                    }
                }
                return _value;
            }
        }

        public static string ResolveConnectionStringFromStorage(string _value)
        {
            if (_value != null && _value.StartsWith("auth="))
            {
                string name = null;
                string pwd = null;

                var pms = _value.Split(';');
                foreach (var pm in pms)
                {
                    if (pm.StartsWith("auth="))
                    {
                        name = pm.Split('=').Skip(1).FirstOrDefault();
                    }

                    if (pm.StartsWith("pwd="))
                    {
                        pwd = pm.Split('=').Skip(1).FirstOrDefault();
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception($"Unable to extract name of stored connection from connection string: {_value}. ");
                }

                while (string.IsNullOrEmpty(pwd))
                {
                    Console.Write($"Please enter password for the connection string storage: ");
                    pwd = GetPassword();
                }

                var secService = new Services.SecurityService();
                var stoService = new Services.AuthStorageService(secService);

                _value = stoService.GetConnectionString(pwd, name);
            }
            return _value;
        }

        public static string GetPassword()
        {
            Console.Write($"Enter password for connection string storage: ");
            StringBuilder input = new StringBuilder();
            while (true)
            {
                int x = Console.CursorLeft;
                int y = Console.CursorTop;
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.SetCursorPosition(x - 1, y);
                    Console.Write(" ");
                    Console.SetCursorPosition(x - 1, y);
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    input.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            return input.ToString();
        }
    }
}
