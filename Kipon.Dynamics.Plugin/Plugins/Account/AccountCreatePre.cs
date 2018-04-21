using Kipon.Dynamics.Plugin.Attributes;

namespace Kipon.Dynamics.Plugin.Plugins.Account
{
    [Step(EventType = CrmEventType.Create, PrimaryEntity = Entities.Account.EntityLogicalName, Stage = StageEnum.PreOperation)]
    public class AccountCreatePre : AbstractBasePlugin
    {
        protected override void Execute(DI.IPluginContext pluginContext)
        {
            var target = pluginContext.Target.ToEntity<Entities.Account>();
            pluginContext.GetService<ServiceAPI.IAccountService>().UppercaseName(target);
        }
    }
}
