using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration.Model
{
    public enum StageEnum
    {
        PreValidate = 10,
        PreOperation = 20,
        PostOperation = 40,
        PostOperationAsyncWithDelete = 41,
        PostOperationAsyncWithoutDelete = 42,
    }
}
