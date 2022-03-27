using Kipon.Xrm.Attributes;
using Kipon.Xrm.Extensions.Sdk;

namespace Kipon.Solid.Plugin.Entities
{
    [TargetFilter(typeof(Model.IPhonenumberChanged), nameof(Account.Telephone1), nameof(Account.Telephone2), nameof(Account.Telephone3))]
    public partial class Account : Model.IPhonenumberChanged
    {
        string[] Model.IPhonenumberChanged.Fields => this.TargetFilterAttributesOf(typeof(Model.IPhonenumberChanged));

    }
}
