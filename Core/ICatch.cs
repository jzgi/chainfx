using System;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    ///
    public interface ICatch
    {
        void Catch(Exception e, ActionContext ac);
    }

    public interface ICatchAsync
    {
        Task CatchAsync(Exception e, ActionContext ac);
    }
}
