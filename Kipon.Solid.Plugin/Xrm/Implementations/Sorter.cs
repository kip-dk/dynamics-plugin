
namespace Kipon.Xrm.Implementations
{
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using System;
    internal class Sorter : IComparable
    {
        public Entity entity { get; private set; }
        private readonly DataCollection<OrderExpression> orderBy;

        internal Sorter(Microsoft.Xrm.Sdk.Entity entity, DataCollection<OrderExpression> orderBy)
        {
            this.entity = entity;
            this.orderBy = orderBy;
        }
        public int CompareTo(object obj)
        {
            if (orderBy != null && orderBy.Count > 0)
            {
                if (obj is Sorter other)
                {
                    foreach (var order in this.orderBy)
                    {
                        var faktor = order.OrderType == OrderType.Ascending ? 1 : -1;
                        var v1 = this.valueOf(order.AttributeName);
                        var v2 = other.valueOf(order.AttributeName);

                        if (v1 == null && v2 == null) continue;
                        if (v1 == null && v2 != null) return -1 * faktor;
                        if (v1 != null && v2 == null) return 1 * faktor;

                        if (v1 is string s1 && v2 is string s2)
                        {
                            var next = string.Compare(s1, s2);
                            if (next != 0)
                            {
                                return next * faktor;
                            }
                        }

                        if (v1 is int i1 && v2 is int i2)
                        {
                            var next = i2 - i1;
                            if (next > 0) return -1 * faktor;
                            if (next < 0) return 1 * faktor;
                            continue;
                        }

                        if (v1 is long long1 && v2 is long long2)
                        {
                            var next = long2 - long1;
                            if (next > 0) return -1 * faktor;
                            if (next < 0) return 1 * faktor;
                            continue;
                        }

                        if (v1 is decimal d1 && v2 is decimal d2)
                        {
                            var next = d2 - d1;
                            if (next > 0M) return -1 * faktor;
                            if (next < 0M) return 1 * faktor;
                            continue;
                        }

                        if (v1 is float f1 && v2 is float f2)
                        {
                            var next = f2 - f1;
                            if (next > 0f) return -1 * faktor;
                            if (next < 0f) return 1 * faktor;
                            continue;
                        }

                        if (v1 is double dob1 && v2 is double dob2)
                        {
                            var next = dob2 - dob1;
                            if (next > 0f) return -1 * faktor;
                            if (next < 0f) return 1 * faktor;
                            continue;
                        }

                        if (v1 is DateTime date1 && v2 is DateTime date2)
                        {
                            if (date1 < date2) return -1 * faktor;
                            if (date1 > date2) return 1 * faktor;
                            continue;
                        }

                        if (v1 is bool bo1 && v2 is bool bo2)
                        {
                            if (bo1 == bo2) continue;
                            if (!bo1) return -1 * faktor;
                            if (bo1) return 1 * faktor;
                        }
                    }
                }
            }
            return 0;
        }

        private object valueOf(string attr)
        {
            if (this.entity.Attributes.ContainsKey(attr))
            {
                var result = this.entity[attr];
                if (result is Microsoft.Xrm.Sdk.EntityReference re)
                {
                    return re.Name;
                }

                if (result is Microsoft.Xrm.Sdk.OptionSetValue ov)
                {
                    return ov.Value;
                }

                if (result is Microsoft.Xrm.Sdk.Money mo)
                {
                    return mo.Value;
                }

                return result;
            }
            return null;
        }
    }
}
