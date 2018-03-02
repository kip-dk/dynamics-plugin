using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration.Entities
{
    public partial class SdkMessageProcessingStep
    {
        /// <summary>
        /// Name/Stage/Rank/Mode/AsyncAutoDelete
        /// </summary>
        public string UniqueName
        {
            get
            {
                return this.Name + "/" + this.Stage.Value + "/" + this.Rank + "/"  + this.Mode.Value.ToString();
            }
        }
    }
}
