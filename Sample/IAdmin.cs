using Greatbone.Core;

namespace Greatbone.Sample
{
    public interface IAdmin
    {
        void search(WebContext wc);

        void del(WebContext wc);

        void status(WebContext wc);
    }
}