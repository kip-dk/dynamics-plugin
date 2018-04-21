namespace Kipon.Dynamics.Plugin.ServiceAPI
{
    public interface IAccountService
    {
        void UppercaseName(Entities.Account target);
        void Reassign(Entities.Account target);
    }
}
