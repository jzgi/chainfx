namespace SkyChain.Db
{
    public interface IPath : IData
    {
        public void Add<V>(V another) where V : struct, IResult;

    }
}