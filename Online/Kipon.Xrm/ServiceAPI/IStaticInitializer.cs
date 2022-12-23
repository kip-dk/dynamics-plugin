namespace Kipon.Xrm.ServiceAPI
{
    /*
     * A static initializer is an interface that can be implemented by a plugin one or several times.
     * It will be called once on first plugin call
     */
    public interface IStaticInitializer
    {
        void Initialize();
    }
}
