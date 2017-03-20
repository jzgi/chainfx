using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// To specify authentication behavior on a service.
    ///
    public interface IAuthenticate
    {
        void Authenticate(ActionContext ac, bool e);
    }

    public interface IAuthenticateAsync
    {
        Task AuthenticateAsync(ActionContext ac, bool e);
    }
}
