using Kipon.Xrm.Fake.Extensions.Query;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.UnitTests.Fake.Extensions.Query
{
    [TestClass]
    public class QueryHelperExtensionsTest : BaseTest
    {
        [TestMethod]
        public void EqualTest()
        {
            {
                var ent1 = new Microsoft.Xrm.Sdk.EntityReference(Entities.Account.EntityLogicalName, Guid.NewGuid());
                var ent2 = new Microsoft.Xrm.Sdk.EntityReference(Entities.Account.EntityLogicalName, ent1.Id);

                Assert.IsTrue(ent1.Equals(ent2));
            }
            {
                object value = "equaltest";
                var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, "equalTEST");
                Assert.IsTrue(value.Equal(filter.Values));
            }

            {
                object value = 10;
                var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 10);
                Assert.IsTrue(value.Equal(filter.Values));
            }

            {
                object value = new Microsoft.Xrm.Sdk.Money(10M);
                var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 10M);
                Assert.IsTrue(value.Equal(filter.Values));
            }

            {
                var value = new Microsoft.Xrm.Sdk.EntityReference(Entities.Contact.EntityLogicalName, Guid.NewGuid());
                var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, value.Id);
                Assert.IsTrue(value.Equal(filter.Values));
            }

            {
                var value = new Microsoft.Xrm.Sdk.EntityReference(Entities.Contact.EntityLogicalName, Guid.NewGuid());
                var v2 = new Microsoft.Xrm.Sdk.EntityReference(Entities.Contact.EntityLogicalName, value.Id);
                var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, v2);
                Assert.IsTrue(value.Equal(filter.Values));
            }

            {
                var value = new Microsoft.Xrm.Sdk.EntityReference(Entities.Contact.EntityLogicalName, Guid.NewGuid());
                var v2 = new Microsoft.Xrm.Sdk.EntityReference(Entities.Account.EntityLogicalName, value.Id);
                var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, v2);
                Assert.IsFalse(value.Equal(filter.Values));
            }

        }


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
        public void EndsWithTest()
        {
            object value = "endswithtest";
            var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith, "TeST");
            Assert.IsTrue(value.EndsWith(filter.Values));

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

        [TestMethod]
        public void ContainsTest()
        {
            object value = "startswithtest";
            var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.Contains, "wItH");
            Assert.IsTrue(value.Contains(filter.Values));
        }

        [TestMethod]
        public void ContainValuesTest()
        {
            var value = new Microsoft.Xrm.Sdk.OptionSetValueCollection();
            value.Add(new Microsoft.Xrm.Sdk.OptionSetValue(1));
            value.Add(new Microsoft.Xrm.Sdk.OptionSetValue(2));
            var filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.ContainValues, new int[] { 1,2 });
            Assert.IsTrue(value.ContainValues(filter.Values));

            filter = new Microsoft.Xrm.Sdk.Query.ConditionExpression("value", Microsoft.Xrm.Sdk.Query.ConditionOperator.ContainValues, new int[] { 1, 2, 3 });
            Assert.IsFalse(value.ContainValues(filter.Values));
        }
    }
}
