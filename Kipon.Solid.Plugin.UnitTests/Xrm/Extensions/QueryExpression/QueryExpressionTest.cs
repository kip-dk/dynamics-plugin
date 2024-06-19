using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.TypeConverters;
using Kipon.Xrm.Extensions.QueryExpression;

namespace Kipon.Solid.Plugin.UnitTests.Xrm.Extensions.QueryExpression
{
    [TestClass]
    public class QueryExpressionTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            {
                var id = Guid.NewGuid();
                var re = new Microsoft.Xrm.Sdk.EntityReference("some", id);
                Assert.AreEqual(id, re.ConvertValueTo<Guid>(out bool r));
            }

            {
                var no = 234;
                var os = new Microsoft.Xrm.Sdk.OptionSetValue(no);
                Assert.AreEqual(no, os.ConvertValueTo<int>(out bool r));
            }
        }


        [TestMethod]
        public void CompareDateTest()
        {
            var d1 = new System.DateTime(2024, 06, 18, 0, 0, 0);
            var d2 = new System.DateTime(2024, 06, 18, 0, 0, 0);

            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal.CompareDate(d1, d2));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual.CompareDate(d1, d2));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual.CompareDate(d1, d2));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.OnOrAfter.CompareDate(d1, d2));
        }

        [TestMethod]
        public void CompareSpecialDateTest()
        {
            var now = System.DateTime.UtcNow;

            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.Today.CompareSpecialDate(null, now));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.ThisWeek.CompareSpecialDate(null, now));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.LastXWeeks.CompareSpecialDate(new object[] { 2 }, now.AddDays(-8)));
        }
    }
}
