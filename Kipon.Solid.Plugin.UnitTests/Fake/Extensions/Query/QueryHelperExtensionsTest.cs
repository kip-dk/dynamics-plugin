extern alias kiponfake;
using kiponfake::Kipon.Xrm.Fake.Extensions.Query;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Fake.Extensions.Query
{
    [TestClass]
    public class QueryHelperExtensionsTest
    {
        [TestMethod]
        public void BeginsWithTest()
        {
            // does start with test
            object value = "startswithtest";
            var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith, "StarTs");
            Assert.IsTrue(value.BeginsWith(filter.Values));

            // null values never match BeginsWith
            value = null;
            Assert.IsFalse(value.BeginsWith(filter.Values));

            // non string never in value is an argument exception
            value = 4536M;
            Assert.ThrowsException<ArgumentException>(() => value.BeginsWith(filter.Values));

            // null in values is an argument exception
            value = "start";
            Assert.ThrowsException<ArgumentException>(() => value.BeginsWith(null));

            // empty values is an argument exception
            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith);
            Assert.ThrowsException<ArgumentException>(() => value.BeginsWith(filter.Values));

            // filter value not a string is an argument exception
            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith, 567M);
            Assert.ThrowsException<ArgumentException>(() => value.BeginsWith(filter.Values));

            // filter value not a string is an argument exception
            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith, new string[] { "", "" });
            Assert.ThrowsException<ArgumentException>(() => value.BeginsWith(filter.Values));
        }

        [TestMethod]
        public void BetweenTest()
        {
            object value = System.DateTime.Now;
            var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Between, new DateTime[] { System.DateTime.Now.AddMinutes(-1), System.DateTime.Now.AddMinutes(1) });
            Assert.IsTrue(value.Between(filter.Values));

            value = System.DateTime.Now;
            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Between, new DateTime[] { System.DateTime.Now.AddSeconds(1), System.DateTime.Now.AddMinutes(1) });
            Assert.IsFalse(value.Between(filter.Values));

            value = 10M;
            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Between, new decimal[] {9M, 11M });
            Assert.IsTrue(value.Between(filter.Values));

            value = 10.0;
            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Between, new double[] { 9.0, 11.0 });
            Assert.IsTrue(value.Between(filter.Values));

            value = 10.0F;
            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Between, new float[] { 9.0F, 11.0F });
            Assert.IsTrue(value.Between(filter.Values));

        }
    }
}
