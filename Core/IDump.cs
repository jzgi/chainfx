namespace Greatbone.Core
{
    ///
    /// The object can be turned from various source contents and to various target contents.
    public interface IDump
    {
        void From(IInput i);

        void To(IOutput o);
    }
}