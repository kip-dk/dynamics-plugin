using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kipon.Solid.Plugin.Plugins.ListMember;

namespace Kipon.Solid.Plugin.UnitTests.Plugins.ListMember
{
    [TestClass]
    public class ListMemberPluginTest : BaseTest
    {
        [TestMethod]
        public void OnPreRemoveMemberTest()
        {
            using (var ctx = Kipon.Xrm.Fake.Repository.PluginExecutionFakeContext.ForType<ListMemberPlugin>())
            {
                ctx.OnPre += delegate ()
                {
                };

                ctx.RemoveMember(ListMemberPlugin.LISTID, ListMemberPlugin.ENTITYID);
            }

        }
    }
}
