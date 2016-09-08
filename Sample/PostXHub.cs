using Greatbone.Core;
using System.Net;

namespace Greatbone.Sample
{
    public class PostXHub : WebXHub
    {
        public PostXHub(WebServiceContext wsc) : base(wsc)
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
            if (wc.Request.GetParam("id", ref id))
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