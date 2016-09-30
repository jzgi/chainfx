using Greatbone.Core;
using System.Net;

namespace Greatbone.Sample
{
    public class PostVarHub : WebVarHub
    {
        public PostVarHub(WebConfig cfg) : base(cfg)
        {
        }

        public void get(WebContext wc, string var)
        {
        }

        public void del(WebContext wc, string var)
        {
        }

        public void addimg(WebContext wc, string var)
        {
        }

        public void img(WebContext wc, string var)
        {
            int id = 0;
            if (wc.GetParam("id", ref id))
            {

            }
            else
            {
                wc.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        public void pub(WebContext wc, long x)
        {
        }
    }
}