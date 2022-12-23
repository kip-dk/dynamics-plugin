using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Online.Plugin.Entities
{
    public partial class Account : Account.IMergedImageForDeleteTest
    {
        public interface IMergedImageForDeleteTest : IAccountMergedimage
        {
            [TargetFilter]
            string AccountNumber { get; }

            string Name { get; }
        }
    }
}
