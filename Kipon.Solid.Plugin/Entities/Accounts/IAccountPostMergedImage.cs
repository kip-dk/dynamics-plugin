using Kipon.Xrm.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account :
        Account.IAccountPostMergedImage
    {

        bool Account.IAccountPostMergedImage.AccountNumberChanged => this.TargetAttributes.ContainsKey("xxx");

        public interface IAccountPostMergedImage : IAccountMergedimage
        {
            [TargetFilter]
            Microsoft.Xrm.Sdk.OptionSetValue AccountRatingCode { get; }

            [TargetFilter]
            string AccountNumber { get; }

            [TargetFilter]
            string Name { get; }

            bool AccountNumberChanged { get; }
        }
    }
}
