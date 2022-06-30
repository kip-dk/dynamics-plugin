using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;
using Kipon.Xrm.Extensions.Sdk;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account: Account.IAccountMergedImageWithTargetAttributes
    {
        public string PreName
        {
            get
            {
                return this.PreimageAttributes?.ValueOf<string>(nameof(Name).ToLower());
            }
        }

        public interface IAccountMergedImageWithTargetAttributes : IAccountMergedimage
        {
            [TargetFilter]
            string Name { get; }

            string AccountNumber { get; }

            string Description { set; }

            string PreName { get; }
        }
    }
}
