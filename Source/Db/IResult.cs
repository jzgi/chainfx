namespace SkyChain.Db
{
    public interface IResult : IData
    {
        public void Add<V>(V another) where V : struct, IResult;

    }
}