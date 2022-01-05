using SkyChain;

namespace SkyChain.Db
{
    public interface IFeature<out P> : IData where P : struct, IFeature<P>
    {
        public P Derive(int ordinal);
    }
}