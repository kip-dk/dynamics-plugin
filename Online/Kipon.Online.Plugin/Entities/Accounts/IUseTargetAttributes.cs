using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Online.Plugin.Entities
{
    public partial class Account : Account.IUseTargetAttributes
    {
        bool IUseTargetAttributes.TelephoneChanged => this.TargetAttributes.ContainsKey(nameof(Telephone1).ToLower());

        string IUseTargetAttributes.PreTelephone1 
        {
            get
            {
                if (this.PreimageAttributes != null && this.PreimageAttributes.ContainsKey(nameof(Telephone1).ToLower()))
                {
                    return this.PreimageAttributes[nameof(Telephone1).ToLower()] as string;
                }
                return null;
            }
        }

        public partial interface IUseTargetAttributes : IAccountMergedimage
        {
            [TargetFilter]
            string Telephone1 { get; }


            string Name { get; }
            string Description { set; }


            bool TelephoneChanged { get; }
            string PreTelephone1 { get; }

        }
    }
}
