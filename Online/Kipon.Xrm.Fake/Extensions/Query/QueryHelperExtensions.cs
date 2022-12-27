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
        public static bool Equal(this object value, Microsoft.Xrm.Sdk.DataCollection<object> values)
        {
            if (values == null || values.Count < 1)
            {
                throw new ArgumentException("Equal expected at least one value in values");
            }

            if (value == null)
            {
                return false;
            }

            var valueType = value.GetType();

            if (valueType == typeof(Guid) || valueType == typeof(Guid?))
            {
                var r = (Guid)value;
                var o = values.Single();
                if (o.GetType() == typeof(Guid))
                {
                    var g = (Guid)o;
                    return r == g;
                }
            }

            if (valueType == typeof(Microsoft.Xrm.Sdk.EntityReference))
            {
                var r = (Microsoft.Xrm.Sdk.EntityReference)value;
                var o = values.Single();
                if (o.GetType() == typeof(Guid))
                {
                    var g = (Guid)o;
                    return r.Id == g;
                }

                if (o.GetType() == typeof(Microsoft.Xrm.Sdk.EntityReference))
                {
                    var g = (Microsoft.Xrm.Sdk.EntityReference)o;
                    return r.LogicalName == g.LogicalName && r.Id == g.Id;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for a reference id compare");
            }

            if (valueType == typeof(Microsoft.Xrm.Sdk.OptionSetValue))
            {
                var r = (Microsoft.Xrm.Sdk.OptionSetValue)value;
                var o = values.Single();
                if (o.GetType() == typeof(int) || o.GetType() == typeof(int?))
                {
                    var i = (int)o;
                    return r.Value == i;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for a optionset compare");
            }

            if (valueType == typeof(Microsoft.Xrm.Sdk.OptionSetValueCollection))
            {
                var r = (Microsoft.Xrm.Sdk.OptionSetValueCollection)value;
                if (r.Count != values.Count)
                {
                    return false;
                }

                var hasValues = r.Select(v => v.Value).ToArray();
                foreach (int i in values)
                {
                    if (!hasValues.Contains(i))
                    {
                        return false;
                    }
                }
                return true;
            }

            if (valueType == typeof(Microsoft.Xrm.Sdk.Money))
            {
                var r = (Microsoft.Xrm.Sdk.Money)value;
                var o = values.Single();
                if (o.GetType() == typeof(decimal) || o.GetType() == typeof(decimal?))
                {
                    var i = (decimal)o;
                    return r.Value == i;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for a Money compare");
            }

            if (valueType == typeof(int) || valueType == typeof(int?))
            {
                var r = (int)value;
                var o = values.Single();
                if (o.GetType() == typeof(int) || o.GetType() == typeof(int?))
                {
                    var i = (int)o;
                    return r == i;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for an int compare");
            }

            if (valueType == typeof(decimal) || valueType == typeof(decimal?))
            {
                var r = (decimal)value;
                var o = values.Single();
                if (o.GetType() == typeof(decimal) || o.GetType() == typeof(decimal?))
                {
                    var i = (decimal)o;
                    return r == i;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for an decimal compare");
            }

            if (valueType == typeof(double) || valueType == typeof(double?))
            {
                var r = (double)value;
                var o = values.Single();
                if (o.GetType() == typeof(double) || o.GetType() == typeof(double?))
                {
                    var i = (double)o;
                    return r == i;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for an double compare");
            }

            if (valueType == typeof(float) || valueType == typeof(float?))
            {
                var r = (float)value;
                var o = values.Single();
                if (o.GetType() == typeof(float) || o.GetType() == typeof(float?))
                {
                    var i = (float)o;
                    return r == i;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for an float compare");
            }

            if (valueType == typeof(bool) || valueType == typeof(bool?))
            {
                var r = (bool)value;
                var o = values.Single();
                if (o.GetType() == typeof(bool) || o.GetType() == typeof(bool?))
                {
                    var i = (bool)o;
                    return r == i;
                }
                throw new ArgumentException($"{o.GetType().FullName} was not expected for an bool compare");
            }

            if (valueType == typeof(string))
            {
                var r = (string)value;
                var o = values.Single();
                if (o.GetType() == typeof(string))
                {
                    var s = (string)o;
                    return r.ToUpperInvariant() == s.ToUpperInvariant();
                }
            }

            return false;
        }
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

        public static bool EndsWith(this object value, Microsoft.Xrm.Sdk.DataCollection<object> values)
        {
            string valueString = null;
            string valuesString = null;
            ResolveStrings(Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith, value, values, out valueString, out valuesString);

            if (valueString == null)
            {
                return false;
            }

            return valueString.ToUpperInvariant().EndsWith(valuesString.ToUpperInvariant());
        }


        public static bool Contains(this object value, Microsoft.Xrm.Sdk.DataCollection<object> values)
        {
            if (values == null || values.Count != 1)
            {
                throw new ArgumentException("Values must contains exact one values");
            }
            if (value == null)
            {
                return false;
            }

            var s = value as string;
            if (s == null)
            {
                throw new ArgumentException("Contains is only supported for fields of type string");
            }

            var containsValue = values.Single() as string;
            if (containsValue == null)
            {
                throw new ArgumentException("expected values to contain a single string");
            }

            return s.ToUpperInvariant().IndexOf(containsValue.ToUpperInvariant()) >= 0;
        }

        public static bool ContainValues(this object value, Microsoft.Xrm.Sdk.DataCollection<object> values)
        {
            if (values == null || values.Count < 1)
            {
                throw new ArgumentException("Values must contains at least one value");
            }

            if (value == null)
            {
                return false;
            }

            var selectedOptions = value as Microsoft.Xrm.Sdk.OptionSetValueCollection;
            if (selectedOptions == null)
            {
                throw new ArgumentException("ContainValues is only supported for OptionSetValueCollection");
            }

            var hasValue = selectedOptions.Select(r => r.Value).ToArray();
            if (hasValue.Length == 0)
            {
                return false;
            }

            foreach (int expect in values)
            {
                if (!hasValue.Contains(expect))
                {
                    return false;
                }
            }

            return true;
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
