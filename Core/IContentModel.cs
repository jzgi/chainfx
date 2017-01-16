namespace Greatbone.Core
{
    public interface IContentModel
    {
        void Dump<R>(ISink<R> snk) where R : ISink<R>;
    }
}