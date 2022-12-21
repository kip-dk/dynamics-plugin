
namespace Kipon.Xrm.Exceptions
{
    public class UnresolvedQueryParameter : BaseException
    {
        public UnresolvedQueryParameter(string name) : base($"Unable to resolve {name} to a query expression.") { }
    }
}
