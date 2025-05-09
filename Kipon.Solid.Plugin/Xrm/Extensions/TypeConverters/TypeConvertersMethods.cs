﻿namespace Kipon.Xrm.Extensions.TypeConverters
{
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Linq;

    public static class TypeConvertersMethods
    {
        [System.Diagnostics.DebuggerNonUserCode()]
        public static Guid ToGuid(this int v1, params int[] others)
        {
            if (v1 > 99999999) throw new ArgumentException("Larges value for v1 is 99999999");
            if (others != null && others.Length >= 1 && others[0] > 9999) throw new ArgumentException("Larges value for v2 is 9999");
            if (others != null && others.Length >= 2 && others[1] > 9999) throw new ArgumentException("Larges value for v3 is 9999");
            if (others != null && others.Length >= 3 && others[2] > 9999) throw new ArgumentException("Larges value for v4 is 9999");

            var result = v1.ToString().PadLeft(8, '0');
            result += "-" + others != null && others.Length >= 1 ? others[0].ToString().PadLeft(4, '0') : "0000";
            result += "-" + others != null && others.Length >= 2 ? others[1].ToString().PadLeft(4, '0') : "0000";
            result += "-" + others != null && others.Length >= 3 ? others[2].ToString().PadLeft(4, '0') : "0000";
            result += "-" + others != null && others.Length == 4 ? others[3].ToString().PadLeft(12, '0') : "000000000000";

            return new Guid(result);
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static Guid ToId(this int value, int typecode)
        {
            return value.ToGuid(0, 0, 0, typecode);
        }

        [System.Diagnostics.DebuggerNonUserCode()]
        public static bool IsSame(this object from, object other)
        {
            if (from == null && other == null) return true;
            if (from != null && other == null) return false;
            if (from == null && other != null) return false;

            {
                if (from is Microsoft.Xrm.Sdk.Money f && other is Microsoft.Xrm.Sdk.Money t)
                {
                    return f.Value == t.Value;
                }
            }

            {
                if (from is Microsoft.Xrm.Sdk.Money f && other is decimal t)
                {
                    return f.Value == t;
                }
            }

            {
                if (from is decimal f && other is Microsoft.Xrm.Sdk.Money t)
                {
                    return f == t.Value;
                }
            }

            {
                if (from is Microsoft.Xrm.Sdk.OptionSetValue f && other is Microsoft.Xrm.Sdk.OptionSetValue t)
                {
                    return f.Value == t.Value;
                }
            }

            {
                if (from is Microsoft.Xrm.Sdk.OptionSetValue f && other is int t)
                {
                    return f.Value == t;
                }
            }

            {
                if (from is int f && other is Microsoft.Xrm.Sdk.OptionSetValue t)
                {
                    return f == t.Value;
                }
            }

            {
                if (from is Microsoft.Xrm.Sdk.EntityReference f && other is Microsoft.Xrm.Sdk.EntityReference t)
                {
                    return f.LogicalName == t.LogicalName && f.Id == t.Id;
                }
            }

            {
                if (from is Microsoft.Xrm.Sdk.EntityReference f && other is Guid t)
                {
                    return f.Id == t;
                }
            }

            {
                if (from is Guid f && other is Microsoft.Xrm.Sdk.EntityReference t)
                {
                    return f == t.Id;
                }
            }


            {
                if (from is decimal f && other is decimal t)
                {
                    return f == t;
                }
            }

            {
                if (from is string f && other is string t)
                {
                    return f == t;
                }
            }

            {
                if (from is int f && other is int t)
                {
                    return f == t;
                }
            }

            {
                if (from is long f && other is long t)
                {
                    return f == t;
                }
            }

            {

                if (from is double f && other is double t)
                {
                    return f == t;
                }
            }

            {
                if (from is float f && other is float t)
                {
                    return f == t;
                }
            }

            {
                if (from is bool f && other is bool t)
                {
                    return f == t;
                }
            }

            return from.Equals(other);
        }


        [System.Diagnostics.DebuggerNonUserCode()]

        public static T ConvertValueTo<T>(this object o, out bool resolved)
        {
            resolved = true;
            if (o == null) return default(T);

            if (o is T t) return t;

            {
                if (o is Microsoft.Xrm.Sdk.EntityReference er && typeof(T) == typeof(Guid))
                {
                    return (T)(object)er.Id;
                }
            }

            {
                if (o is Microsoft.Xrm.Sdk.OptionSetValue er && typeof(T) == typeof(int))
                {
                    return (T)(object)er.Value;
                }
            }

            {
                if (o is Microsoft.Xrm.Sdk.Money er && typeof(T) == typeof(decimal))
                {
                    return (T)(object)er.Value;
                }
            }


            resolved = false;
            return default(T);
        }

        public static bool ToBool(this object value)
        {
            if (value == null)
            {
                throw new InvalidPluginExecutionException("null cannot be converted to bool");
            }

            if (value is bool b)
            {
                return b;
            }

            var co = value.ToString().ToUpper();
            return co == "TRUE" || co == "YES" || co == "ON" || co == "JA" || co == "1";
        }

        public static int ToInt(this string value)
        {
            if (int.TryParse(value, out int v))
            {
                return v;
            }

            throw new InvalidPluginExecutionException($"[{ value }] cannot be converted to whole number");
        }

        public static int ExtractConditionValue(this string expression)
        {
            return int.Parse(expression.Split('(').Last().Replace(")", ""));
        }

    }
}
