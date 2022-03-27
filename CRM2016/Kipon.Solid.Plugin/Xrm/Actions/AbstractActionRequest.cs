namespace Kipon.Xrm.Actions
{
    using System;
    public abstract class AbstractActionRequest
    {
        private readonly Microsoft.Xrm.Sdk.IPluginExecutionContext ctx;
        public AbstractActionRequest(Microsoft.Xrm.Sdk.IPluginExecutionContext ctx)
        {
            this.ctx = ctx;
        }


        protected T ValueOf<T>(string name)
        {
            if (ctx.InputParameters.ContainsKey(name))
            {
                var value = ctx.InputParameters[name];
                return (T)value;
            }
            return default(T);
        }
    }
}
