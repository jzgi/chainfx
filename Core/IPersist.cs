namespace Greatbone.Core
{
    ///
    /// The object can be turned from various source contents and to various target contents.
    public interface IPersistable
    {
        void From(ISource s);

        void To(ITarget t);
    }
}