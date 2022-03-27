using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd
{
    public interface ICmd
    {
        Task ExecuteAsync(string[] args);
    }
}
