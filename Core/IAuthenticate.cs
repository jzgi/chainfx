using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// <summary>
    /// To implement authentication behavior on a service class.
    /// </summary>
    public interface IAuthenticate
    {
        bool Authenticate(ActionContext ac, bool e);
    }

    /// <summary>
    /// To implement asynchronous authentication behavior on a service class.
    /// </summary>
    public interface IAuthenticateAsync
    {
        Task<bool> AuthenticateAsync(ActionContext ac, bool e);
    }
}