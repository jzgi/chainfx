using Greatbone.Core;
using System.Net;
using System.Collections.Generic;

namespace Greatbone.Sample
{
    public class PostHub : WebHub
    {
        public PostHub(WebConfig cfg) : base(cfg)
        {
            SetVarHub<PostVarHub>(false);
        }

        public void Top(WebContext wc)
        {
            int page = 0;
            if (wc.GetParam("page", ref page))
            {
                wc.Response.StatusCode = 400;
                return;
            }

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Query(@"SELECT * FROM posts ORDER BY id DESC LIMIT @limit OFFSET @offset",
                    p => p.Put("@limit", 20).Put("@offset", 20 * page)))
                {
                    List<Post> list = null;
                    while (dc.NextRow())
                    {
                    }
                    wc.Response.StatusCode = (int) HttpStatusCode.OK;
                    // wc.Response.SetContent(list);
                }
                else
                {
                    wc.Response.StatusCode = (int) HttpStatusCode.NoContent;
                }
            }
        }


        public void New(WebContext wc)
        {
            IToken tok = wc.Token;

            using (var dc = Service.NewSqlContext())
            {
                dc.Execute("INSERT INTO posts () VALUES ()",
                    p =>
                    {
                        p.Put("@authorid", tok.Key);
                        p.Put("@author", tok.Name);
                    }
                );
            }
        }

        public void Remove(WebContext wc)
        {
        }
    }
}