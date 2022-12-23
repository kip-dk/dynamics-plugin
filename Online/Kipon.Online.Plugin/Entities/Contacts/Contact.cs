using Kipon.Xrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Entities
{
    public partial class Contact : Contact.INameChanged, Contact.IPreName, Model.IProspect, Model.INamed, Kipon.Xrm.IReplaceEntityReferenceEmptyGuidWithNull
    {

        [Microsoft.Xrm.Sdk.AttributeLogicalName("fullname")]
        public string Name
        {
            get
            {
                return this.FullName;
            }
        }

        string[] IReplaceEntityReferenceEmptyGuidWithNull.ForAttributes => null;

        public interface INameChanged : IContactTarget
        {
            string FirstName { get; set; }
            string LastName { get; set; }
            string FullName { get; }
        }


        public interface IPreName : IContactPreimage
        {
            string FirstName { get;  }
            string LastName { get; }
            Microsoft.Xrm.Sdk.Money CreditLimit { get;  }
        }
    }
}
