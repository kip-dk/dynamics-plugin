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

            if (value == null)
            {
                return false;
            }

            if (value.GetType() == typeof(System.DateTime) || value.GetType() == typeof(System.DateTime?))
            {
                var o = (DateTime)value;
                var v1 = (DateTime)values.First();
                var v2 = (DateTime)values.Last();
                return o >= v1 && o <= v2;
            }

            if (value.GetType() == typeof(decimal) || value.GetType() == typeof(decimal?))
            {
                var o = (decimal)value;
                var v1 = (decimal)values.First();
                var v2 = (decimal)values.Last();
                return o >= v1 && o <= v2;
           }

            if (value.GetType() == typeof(Microsoft.Xrm.Sdk.Money))
            {
                var o = (Microsoft.Xrm.Sdk.Money)value;
                var v1 = (decimal)values.First();
                var v2 = (decimal)values.Last();
                return o.Value >= v1 && o.Value <= v2;
            }


            if (value.GetType() == typeof(double) || value.GetType() == typeof(double?))
            {
                var o = (double)value;
                var v1 = (double)values.First();
                var v2 = (double)values.Last();
                return o >= v1 && o <= v2;
            }

            if (value.GetType() == typeof(float) || value.GetType() == typeof(float?))
            {
                var o = (float)value;
                var v1 = (float)values.First();
                var v2 = (float)values.Last();
                return o >= v1 && o <= v2;
            }


            if (value.GetType() == typeof(int) || value.GetType() == typeof(int?))
            {
                var o = (int)value;
                var v1 = (int)values.First();
                var v2 = (int)values.Last();
                return o >= v1 && o <= v2;
            }
            throw new ArgumentException($"Unexpected type in between clause {value.GetType().FullName}");
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
