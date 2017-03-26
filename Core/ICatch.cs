using System;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// To implement exception handling on a service class.
    ///
    public interface ICatch
    {
        void Catch(Exception e, ActionContext ac);
    }

    /// To implement asynchronous exception handling on a service class.
    ///
    public interface ICatchAsync
    {
        Task CatchAsync(Exception e, ActionContext ac);
    }
}
