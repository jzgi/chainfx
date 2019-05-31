using System.Threading.Tasks;

namespace Greatbone.Web
{
    public interface IPollContext
    {
        string RefShard { get; }

        string RefName { get; }

        string QueryString { get; set; }

        Task<byte[]> PollAsync();

        Task<M> PollAsync<M>() where M : class, ISource;

        Task<D> PollObjectAsync<D>(byte proj = 0x0f) where D : IData, new();

        Task<D[]> PollArrayAsync<D>(byte proj = 0x0f) where D : IData, new();
    }
}