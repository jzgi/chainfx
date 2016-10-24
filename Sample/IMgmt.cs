using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The general management interface.
    /// </summary>
    ///
    public interface IMgmt
    {
        void srch(WebContext wc, string subscpt);

        void del(WebContext wc, string subscpt);

        void status(WebContext wc, string subscpt);
    }
}