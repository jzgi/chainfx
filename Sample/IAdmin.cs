using Greatbone.Core;

namespace Greatbone.Sample
{
    public interface IAdmin
    {
        void search(WebContext wc, string subscpt);

        void del(WebContext wc, string subscpt);

        void status(WebContext wc, string subscpt);
    }
}