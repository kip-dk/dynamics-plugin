using Kipon.Xrm.Tools.Extensions.Strings;
using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Kipon.Xrm.Tools.Services
{
    [Export(typeof(ServiceAPI.IAuthStorageService))]
    public class AuthStorageService : ServiceAPI.IAuthStorageService
    {
        private readonly ISecurityService secService;

        [ImportingConstructor]
        public AuthStorageService(ServiceAPI.ISecurityService secService)
        {
            this.secService = secService;
        }

        public void Create(string pwd, string name, string connectionString)
        {
            var models = this.GetModel(pwd, false);
            var exists = models.Where(r => r.Name == name).SingleOrDefault();
            if (exists != null)
            {
                throw new Exception($"{ name } already exists. Use update to change the connection string");
            }

            var all = new List<Models.Auth>();
            if (models.Length > 0)
            {
                all.AddRange(models);
            }

            all.Add(new Models.Auth { Name = name, ConnectionString = connectionString });
            this.SaveModels(pwd, all.ToArray());

            Console.WriteLine($"Connectionstring: { name } was added to storage");
        }

        public void Update(string pwd, string name, string connectionString)
        {
            var models = this.GetModel(pwd, true);

            var exists = models.Where(r => r.Name == name).SingleOrDefault();
            if (exists == null)
            {
                throw new Exception($"No connection string with {name} exists. Use create to create a connection string");
            }

            exists.ConnectionString = connectionString;
            this.SaveModels(pwd, models);

        }

        public void Delete(string pwd, string name)
        {
            var models = this.GetModel(pwd, true);

            var exists = models.Where(r => r.Name == name).SingleOrDefault();
            if (exists == null)
            {
                throw new Exception($"No connection string with {name} exists. Unable to delete connect string");
            }
            models = models.Where(r => r.Name != name).ToArray();
            this.SaveModels(pwd, models);
        }

        public void List(string pwd)
        {
            var models = this.GetModel(pwd, true);
            foreach (var model in models)
            {
                Console.WriteLine($"{ model.Name.PadRight(20) }: { model.ConnectionString }");
            }
        }

        public string GetConnectionString(string pwd, string name)
        {
            var models = this.GetModel(pwd, true);
            var match = models.Where(r => r.Name == name).SingleOrDefault();

            if (match == null)
            {
                throw new Exception($"No connection string named: { name } was found in current users storage");
            }

            return match.ConnectionString;
        }


        public static readonly string AUTH_FILE_NAME = $@"{ Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) }\.kipon.xrm.keystorage";

        private Models.Auth[] GetModel(string pwd, bool throwIfNowFound = true)
        {
            if (System.IO.File.Exists(AUTH_FILE_NAME))
            {
                var content = this.secService.Decrypt(pwd, System.IO.File.ReadAllText(AUTH_FILE_NAME));
                var bytes = System.Text.Encoding.UTF8.GetBytes(content);

                using (var str = new System.IO.MemoryStream(bytes))
                {
                    var ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Models.Auth[]));
                    return (Models.Auth[])ser.ReadObject(str);
                }
            } else
            {
                if (throwIfNowFound)
                {
                    throw new System.IO.FileNotFoundException(AUTH_FILE_NAME);
                }
                return new Models.Auth[0];
            }
        }

        private void SaveModels(string pwd, Models.Auth[] models)
        {
            if (models.Length == 0)
            {
                System.IO.File.Delete(AUTH_FILE_NAME);
                Console.WriteLine($"Kipon Xrm Auth file was deleted. No auth was registred anymore.");
                return;
            }

            var content = secService.Encrypt(pwd, models.ToJsonString());

            System.IO.File.WriteAllText(AUTH_FILE_NAME,  content);
        }
    }
}
