using System;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    ///
    ///
    public interface ICatch
    {
        void Catch(ActionContext ac, Exception e);
    }

    public interface ICatchAsync
    {
        Task CatchAsync(ActionContext ac, Exception e);
    }
}
