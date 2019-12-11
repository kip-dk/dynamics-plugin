using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Extensions.Query
{
    public static class QueryHelperExtensions
    {
        internal static string[] ToAttributNames(this Microsoft.Xrm.Sdk.Query.ColumnSet columnSet)
        {
            if (columnSet == null)
            {
                return new string[0];
            }

            if (columnSet.AllColumns)
            {
                return null;
            }

            if (columnSet.Columns == null)
            {
                return new string[0];
            }

            return columnSet.Columns.ToArray();
        }

        #region query expressions
        public static bool BeginsWith(this object value, Microsoft.Xrm.Sdk.DataCollection<object> values)
        {
            string valueString = null;
            string valuesString = null;
            ResolveStrings(Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith, value, values, out valueString, out valuesString);

            if (valueString == null)
            {
                return false;
            }

            return valueString.ToUpperInvariant().StartsWith(valuesString.ToUpperInvariant());
        }

        public static bool Between(this object value, Microsoft.Xrm.Sdk.DataCollection<object> values)
        {
            if (values == null || values.Count != 2)
            {
                throw new ArgumentException("Values must contains exact two values, lower and upper boundary");
            }

#warning TO-DO implement
            return true;
        }

        private static void ResolveStrings(Microsoft.Xrm.Sdk.Query.ConditionOperator opr, object value, Microsoft.Xrm.Sdk.DataCollection<object> values, out string valueString, out string valuesString)
        {
            if (values == null || values.Count != 1)
            {
                throw new ArgumentException("Begins with is expecting values to be a single string");
            }

            valuesString = values[0] as string;
            if (valuesString == null)
            {
                throw new ArgumentException("Begins with is expecting values to be a single string");
            }


            valueString = value as string;

            if (value != null && valueString == null)
            {
                throw new ArgumentException($"Value pased, expected a string but got {value.GetType().FullName}");
            }
        }
        #endregion
    }
}
