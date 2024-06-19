﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void CompareStringTest()
        {
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal.CompareString("a string", "a string"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith.CompareString("a string", "a string that starts with"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.Contains.CompareString("a string", "this string does contain a string just somewhere"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotBeginWith.CompareString("a string", "this string does not begin with a string"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotContain.CompareString("a string", "this string does not begin contain the a-string."));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.DoesNotEndWith.CompareString("a string", "this string does not end with a string. We do have something after"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith.CompareString("a string", "this string end with a string. We do have something after. a string"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.Like.CompareString("a string", "this string end with a string. We do have something after. a string"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual.CompareString("a string", "b string"));
            Assert.IsTrue(Microsoft.Xrm.Sdk.Query.ConditionOperator.NotLike.CompareString("a string", "b string"));
        }

        [TestMethod]
        public void QuickFindMatchTest()
        {
            var accounts = new Entities.Account[]
            {
                new Entities.Account{ Name = "Kipon ApS", Description = "En beskrivelse", PrimaryContactId = new Microsoft.Xrm.Sdk.EntityReference{ Id = Guid.NewGuid(), LogicalName = Entities.Contact.EntityLogicalName, Name = "Kjeld Ingemann Poulsen" } }
            };

            Assert.IsTrue(accounts[0].QuickFindMatch("En be%", new string[] { "name", "description", "primarycontactid" }));
            Assert.IsTrue(accounts[0].QuickFindMatch("Kipon%", new string[] { "name", "description", "primarycontactid" }));
            Assert.IsTrue(accounts[0].QuickFindMatch("%ingemann%", new string[] { "name", "description", "primarycontactid" }));
            Assert.IsTrue(accounts[0].QuickFindMatch(" aps", new string[] { "name", "description", "primarycontactid" }));
        }
    }
}
