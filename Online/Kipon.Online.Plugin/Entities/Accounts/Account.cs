using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Solid.Plugin.Model;
using Kipon.Xrm;
using Kipon.Xrm.Attributes;
using Microsoft.Xrm.Sdk;
using Kipon.Xrm.Extensions.Sdk;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class Account : Model.IProspect, Model.INamed, IReplaceEntityReferenceEmptyGuidWithNull
    {
        [Microsoft.Xrm.Sdk.AttributeLogicalName("saldo")]
        public Microsoft.Xrm.Sdk.Money Saldo { get; set; }


        [Microsoft.Xrm.Sdk.AttributeLogicalName("accountcategorycode")]
        public AnEnum? AnEnumValue
        {
            get
            {
                if (this.AccountCategoryCode != null)
                {
                    return (AnEnum)this.AccountCategoryCode.Value;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    this.AccountCategoryCode = new OptionSetValue((int)value.Value);
                } else
                {
                    this.AccountCategoryCode = null;
                }
            }
        }

        public string NoDecorationProperty { get; set; }

        private static readonly string[] EMPTYGUIDSFIELDS = new string[]
        {
            nameof(ParentAccountId).ToLower(),
            nameof(PrimaryContactId).ToLower(),
        };

        string[] IReplaceEntityReferenceEmptyGuidWithNull.ForAttributes => EMPTYGUIDSFIELDS;

        public enum AnEnum
        {
            Value1 = 1,
            Value2 = 2
        }
    }
}
