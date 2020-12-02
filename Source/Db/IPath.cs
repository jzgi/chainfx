namespace SkyChain.Db
{
    public interface IPath<out P> : IData where P : struct, IPath<P>
    {
        public P Derive(int ordinal);
    }
}