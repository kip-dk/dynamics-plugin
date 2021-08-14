using Kipon.Xrm.Tools.ServiceAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Cmd.Tools
{
    [Export("wr-generate", typeof(ICmd))]
    public class WebResourceGenerateDefinitions : ICmd
    {
        private readonly ISystemFormService formService;
        private readonly IMessageService messageService;

        [ImportingConstructor]
        public WebResourceGenerateDefinitions(Xrm.Tools.ServiceAPI.ISystemFormService formService, Xrm.Tools.ServiceAPI.IMessageService messageService)
        {
            this.formService = formService ?? throw new ArgumentNullException(nameof(formService));
            this.messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        }

        public Task ExecuteAsync(string[] args)
        {
            if (System.IO.File.Exists("filter.xml"))
            {
                using (var file = new System.IO.FileStream("filter.xml", System.IO.FileMode.Open))
                {
                    var config = new Kipon.Xrm.Tools.Configs.Webresource(file);
                    using (var output = new System.IO.FileStream($@"{config.Forms.Typings}{config.Forms.Namespace}.d.ts", System.IO.FileMode.Create))
                    {
                        using (var tsWriter = new Kipon.Xrm.Tools.CodeWriter.Typescript.Writer(output))
                        {
                            tsWriter.Write(config.Forms, this.formService);
                        }
                    }
                }
            }
            else
            {
                this.messageService.Inform($"Could not find file: filter.xml in current directory.");
            }
            return Task.CompletedTask;
        }
    }
}
