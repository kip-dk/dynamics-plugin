using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface ISecurityService
    {
        string Encrypt(string pwd, string content);
        string Decrypt(string pwd, string content);
    }
}
