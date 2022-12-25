using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.ServiceAPI
{
    public interface IFileService
    {
        void Upload(string filename, byte[] data, Microsoft.Xrm.Sdk.EntityReference targetId, string attribLogicalName);
    }
}
