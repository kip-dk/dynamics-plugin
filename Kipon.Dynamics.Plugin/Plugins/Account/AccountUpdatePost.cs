using Kipon.Dynamics.Plugin.Attributes;

namespace Kipon.Dynamics.Plugin.Plugins.Account
{
    [Step(EventType = CrmEventType.Update, PrimaryEntity = Entities.Account.EntityLogicalName, Stage = StageEnum.PostOperation)]
    public class AccountUpdatePost : AbstractBasePlugin
    {
        protected override void Execute(DI.IPluginContext pluginContext)
        {
            var target = pluginContext.Target.ToEntity<Entities.Account>();

            if (pluginContext.AttributeChanged("description") && !string.IsNullOrEmpty(target.Description))
            {
                var accountService = pluginContext.GetService<ServiceAPI.IAccountService>();
                accountService.Reassign(target);
            }
        }
    }
}
