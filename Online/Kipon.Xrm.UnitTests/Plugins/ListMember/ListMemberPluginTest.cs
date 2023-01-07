using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Online.Plugin.Plugins.ListMember;

namespace Kipon.Xrm.UnitTests.Plugin.UnitTests.Plugins.ListMember
{
    [TestClass]
    public class ListMemberPluginTest : BaseTest
    {
        [TestMethod]
        public void OnPreRemoveMemberTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<ListMemberPlugin>())
            {
                var called = false;
                ctx.OnPre = delegate ()
                {
                    called = true;
                };

                ctx.RemoveMember(ListMemberPlugin.LISTID, ListMemberPlugin.ENTITYID);

                Assert.IsTrue(called);
            }

        }
    }
}
