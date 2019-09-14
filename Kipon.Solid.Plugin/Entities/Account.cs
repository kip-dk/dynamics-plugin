using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : Microsoft.Xrm.Sdk.Entity,
        IAccountTarget,
        IAccountPreimage,
        IAccountPostimage,
        IAccountMergedimage,
        IAccountNameChanged
    {
        public string Name { get; set; }

    }

    public interface IAccountNameChanged : IAccountTarget
    {
        string Name { get; set; }
    }

    public partial interface IAccountTarget : Kipon.Solid.Plugin.Xrm.Target<Account> { }
    public partial interface IAccountPreimage : Kipon.Solid.Plugin.Xrm.Preimage<Account> { }
    public partial interface IAccountPostimage : Kipon.Solid.Plugin.Xrm.Postimage<Account> { }
    public partial interface IAccountMergedimage : Kipon.Solid.Plugin.Xrm.Mergedimage<Account> { }
}
