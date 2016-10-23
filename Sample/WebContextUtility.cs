using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public static class WebContextUtility
    {

        public static void OutPrimeHt(this WebContext wc, int status, string header, Action<AbsHtContent> main, bool? pub = null, int maxage = 60000)
        {
            AbsHtContent cont = new AbsHtContent(header, true);
            cont.Render(main);
            wc.Out(status, cont, pub, maxage);
        }

        public static void OutDialogHt(this WebContext wc, int status, string header, Action<AbsHtContent> main, bool? pub = null, int maxage = 60000)
        {
            AbsHtContent cont = new AbsHtContent(null, false);
            cont.Render(main);
            wc.Out(status, cont, pub, maxage);
        }

        public static void OutFeedHt(this WebContext wc, int status, string header, Action<AbsHtContent> main, bool? pub = null, int maxage = 60000)
        {
            AbsHtContent cont = new AbsHtContent(header, false);
            cont.Render(main);
            wc.Out(status, cont, pub, maxage);
        }

    }
}