using SkyChain;

namespace SkyChain.Chain
{
    public interface IOutcome<in R> : IData where R : struct, IOutcome<R>
    {
        public void OnAdd(R another);
    }
}