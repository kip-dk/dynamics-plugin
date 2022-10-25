using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Xrm.Extensions.TypeConverters;

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
    }
}
