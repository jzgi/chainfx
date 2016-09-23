using Greatbone.Core;
using System.Net;

namespace Greatbone.Sample
{
    public class PostVarHub : WebVarHub
    {
        public PostVarHub(WebConfig cfg) : base(cfg)
        {
        }

        public void Get(WebContext wc, long x)
        {
        }

        public void Delete(WebContext wc, long x)
        {
        }

        public void AddImg(WebContext wc, long x)
        {
        }

        public void Img(WebContext wc, long x)
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

        public void Publish(WebContext wc, long x)
        {
        }
    }
}