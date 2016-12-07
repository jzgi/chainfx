namespace Greatbone.Core
{
    ///
    /// A context that can initiate remote call.
    ///
    public interface ICallerContext
    {
        IPrincipal Principal { get; }

        bool IsBearer { get; }
    }
}
