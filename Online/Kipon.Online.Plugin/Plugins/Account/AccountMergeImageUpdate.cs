using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Online.Plugin.Plugins.Account
{
    public class AccountMergeImageUpdate : Kipon.Xrm.BasePlugin
    {
        public const string TEST_VALUE = "Merge test name";

        public void OnPreUpdate(Entities.Account target, Entities.Account mergedimage)
        {
            if (target.Name == TEST_VALUE)
            {
                mergedimage.Description = $"Assigned: {TEST_VALUE}";

                if (target.Description != $"Assigned: {TEST_VALUE}")
                {
                    throw new InvalidPluginExecutionException($"Description assign did not populate to target as expected.");
                }

            }

            if (mergedimage.AccountNumber == TEST_VALUE)
            {
                target.AccountNumber = $"Assigned: {TEST_VALUE}";

                if (mergedimage.AccountNumber != $"Assigned: {TEST_VALUE}")
                {
                    throw new InvalidPluginExecutionException("AccountNumber did not reflect to mergedimage as expected");
                }
            }
        }
    }
}
