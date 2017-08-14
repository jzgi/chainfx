using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// To implement authentication behavior on a service class.
    ///
    public interface IAuthenticate
    {
        bool Authenticate(ActionContext ac, bool e);
    }

    /// To implement asynchronous authentication behavior on a service class.
    ///
    public interface IAuthenticateAsync
    {
        Task<bool> AuthenticateAsync(ActionContext ac, bool e);
    }
}