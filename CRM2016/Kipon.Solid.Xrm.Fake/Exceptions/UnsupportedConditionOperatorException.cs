using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Exceptions
{
    public class UnsupportedConditionOperatorException : BaseException
    {
        public UnsupportedConditionOperatorException(Microsoft.Xrm.Sdk.Query.ConditionOperator conditionop) : base($"{conditionop.ToString()}") { }
    }
}
