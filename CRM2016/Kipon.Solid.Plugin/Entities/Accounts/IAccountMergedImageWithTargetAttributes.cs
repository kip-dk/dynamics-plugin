using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;
namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account: Account.IAccountMergedImageWithTargetAttributes
    {
        public interface IAccountMergedImageWithTargetAttributes : IAccountMergedimage
        {
            [TargetFilter]
            string Name { get; }

            string AccountNumber { get; }

            string Description { set; }
        }
    }
}
