using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IAuthStorageService
    {
        void Create(string pwd, string name, string connectionString);
        void Update(string pwd, string name, string connectionString);
        void Delete(string pwd, string name);
        void List(string pwd);
        string GetConnectionString(string pwd, string name);
    }
}
