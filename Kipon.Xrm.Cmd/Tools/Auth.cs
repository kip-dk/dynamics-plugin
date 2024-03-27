using Kipon.Xrm.Tools.Extensions.Strings;
using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("auth", typeof(ICmd))]
    public class Auth : ICmd
    {
        private readonly IAuthStorageService authStorageService;

        [ImportingConstructor]
        public Auth(Kipon.Xrm.Tools.ServiceAPI.IAuthStorageService authStorageService)
        {
            this.authStorageService = authStorageService;
        }

        public Task ExecuteAsync(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine($"Usage: Kipon.Xrm.Cmd auth [create | update | delete | list]");
                return Task.CompletedTask;
            }

            var pwd = "pwd".GetCommandlineParameter();
            while (string.IsNullOrEmpty(pwd))
            {
                Console.Write($"Please enter password: ");
                pwd = Console.ReadLine();
            }

            switch (args[0])
            {
                case "create":
                    {
                        var name = "name".GetCommandlineParameter();

                        while (string.IsNullOrEmpty(name))
                        {
                            Console.Write($"Please enter name of connection: ");
                            name = Console.ReadLine();
                        }

                        var con = "connectionstring".GetCommandlineParameter();
                        while (string.IsNullOrEmpty(con))
                        {
                            Console.Write($"Pleaster enter connectionstring: ");
                            con = Console.ReadLine();
                        }

                        this.authStorageService.Create(pwd, name, con);
                        break;
                    }
                case "update":
                    {
                        var name = "name".GetCommandlineParameter();

                        while (string.IsNullOrEmpty(name))
                        {
                            Console.Write($"Please enter name of connection: ");
                            name = Console.ReadLine();
                        }

                        var con = "connectionstring".GetCommandlineParameter();
                        while (string.IsNullOrEmpty(con))
                        {
                            Console.Write($"Pleaster enter connectionstring: ");
                            con = Console.ReadLine();
                        }

                        this.authStorageService.Update(pwd, name, con);
                        break;
                    }
                case "delete":
                    {
                        var name = "name".GetCommandlineParameter();

                        while (string.IsNullOrEmpty(name))
                        {
                            Console.Write($"Please enter name of connection: ");
                            name = Console.ReadLine();
                        }

                        this.authStorageService.Delete(pwd, name);
                        break;

                    }
                case "list":
                    {
                        this.authStorageService.List(pwd);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Usage: Kipon.Xrm.Cmd auth [create | update | delete | list]");
                        return Task.CompletedTask; ;
                    }
            }

            return Task.CompletedTask;
        }
    }
}
