using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.PluginRegistration.Entities
{
    public partial class SdkMessageProcessingStepImage
    {
        /// <summary>
        /// Virtual property to manage if the preimage is still relevant
        /// </summary>
        public bool Relevant { get; set; }
    }
}
