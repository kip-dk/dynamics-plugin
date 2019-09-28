using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Attributes;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : ICreditLimitChanged, IAccountNameChanged
    {
        public Microsoft.Xrm.Sdk.Money Saldo { get; set; }
    }

    public interface IAccountNameChanged : IAccountTarget
    {
        [Required]
        string Name { get; set; }
    }

    public interface ICreditLimitChanged : IAccountTarget
    {
        Microsoft.Xrm.Sdk.Money CreditLimit { get; }
    }

    public interface IForgotToImplement : IAccountTarget
    {
    }
}
