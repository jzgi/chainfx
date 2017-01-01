namespace Greatbone.Core
{
    ///
    /// A context that can initiate remote call.
    ///
    public interface ICallerContext
    {
        IToken Token { get; }

        string TokenStr { get; }

        bool Cookied { get; }
    }
}
