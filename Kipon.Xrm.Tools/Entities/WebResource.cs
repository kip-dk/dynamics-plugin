using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Entities
{
    public partial class WebResource
    {
        [Microsoft.Xrm.Sdk.AttributeLogicalName("webresourcetype")]
        public WebResourceTypeEnum? Type
        {
            get
            {
                if (this.WebResourceType != null)
                {
                    return (WebResourceTypeEnum)this.WebResourceType.Value;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    this.WebResourceType = new Microsoft.Xrm.Sdk.OptionSetValue((int)value.Value);
                } else
                {
                    this.WebResourceType = null;
                }
            }
        }
    }
}
