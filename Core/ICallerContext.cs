namespace Greatbone.Core
{
    ///
    /// To initiate remote call.
    ///
    public interface ICallerContext
    {
        IPrincipal Principal { get; }

        bool IsBearer { get; }
    }
}
