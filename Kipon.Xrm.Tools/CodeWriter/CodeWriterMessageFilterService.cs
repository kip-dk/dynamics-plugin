using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Services.Utility;


namespace Kipon.Xrm.Tools.CodeWriter
{
    public class CodeWriterMessageFilterService : ICodeWriterMessageFilterService
    {
        private readonly ICodeWriterMessageFilterService defService;

        public CodeWriterMessageFilterService(ICodeWriterMessageFilterService defService)
        {
            this.defService = defService;
        }

        public bool GenerateSdkMessage(SdkMessage sdkMessage, IServiceProvider services)
        {
            if (sdkMessage.Name != null && sdkMessage.Name.Contains("UpsertMultiple"))
            {
                return false;
            }

            return defService.GenerateSdkMessage(sdkMessage, services);
        }

        public bool GenerateSdkMessagePair(SdkMessagePair sdkMessagePair, IServiceProvider services)
        {
            if (sdkMessagePair.Message != null && sdkMessagePair.Message != null && sdkMessagePair.Message.Name != null && sdkMessagePair.Message.Name.Contains("UpsertMultiple"))
            {
                return false;
            }
            return defService.GenerateSdkMessagePair(sdkMessagePair, services);
        }
    }
}
