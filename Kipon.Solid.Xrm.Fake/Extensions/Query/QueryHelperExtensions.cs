using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Extensions.Query
{
    internal static class QueryHelperExtensions
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
    }
}
