namespace Kipon.Xrm
{
    using Microsoft.Xrm.Sdk;
    using System;

    public interface IStepExecuter
    {
        void Run(IServiceProvider serviceProvider, IPluginExecutionContext ctx, Action invokeAutoMappedStep);
    }
}
