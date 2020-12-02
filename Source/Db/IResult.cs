namespace SkyChain.Db
{
    public interface IResult<in R> : IData where R : struct, IResult<R>
    {
        public void OnAdd(R another);
    }
}