namespace Greatbone.Core
{

    /// <summary>
    /// Represents a persistable object that follows the data input/ouput convention.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource s, byte x = 0xff);

        void Dump<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>;
    }

}