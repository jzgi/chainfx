namespace Greatbone.Core
{

    /// <summary>
    /// Represents a data object that follows certain input/ouput conventions.
    /// </summary>
    public interface IBean
    {
        void Load(ISource s, byte z = 0);

        void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>;
    }

}