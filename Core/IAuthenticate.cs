using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// To specify authentication behavior on a service.
    ///
    public interface IAuthenticate
    {
        bool Authenticate(ActionContext ac, bool e);
    }

    public interface IAuthenticateAsync
    {
        Task<bool> AuthenticateAsync(ActionContext ac, bool e);
    }
}
