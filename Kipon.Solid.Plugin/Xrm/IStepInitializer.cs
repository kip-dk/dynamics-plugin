namespace Kipon.Xrm
{
    using Microsoft.Xrm.Sdk;
    using System;

    public interface IStepInitializer
    {
        void Initialize(IServiceProvider serviceProvider);
    }
}
