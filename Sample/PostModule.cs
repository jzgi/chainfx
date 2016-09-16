using Greatbone.Core;
using System.Net;
using System.Collections.Generic;

namespace Greatbone.Sample
{
    public class PostModule : WebModule
    {
        public PostModule(WebSubConfig cfg) : base(cfg)
        {
            SetXHub<PostXHub>(false);
        }

        public void Top(WebContext wc)
        {
            int page = 0;
            wc.Request.GetParam("page", ref page);

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Query(@"SELECT * FROM posts ORDER BY id DESC LIMIT @limit OFFSET @offset", p =>
                {
                    p.Set("@limit", 20);
                    p.Set("@offset", 20 * page);
                }))
                {

                    List<Post> list = null;
                    while (dc.NextRow())
                    {

                    }
                    wc.Response.StatusCode = (int)HttpStatusCode.OK;
                    // wc.Response.SetContent(list);
                }
                else
                {
                    wc.Response.StatusCode = (int)HttpStatusCode.NoContent;
                }
            }
        }


        public void New(WebContext wc)
        {
            IToken tok = wc.Token;

            Post o = wc.Request.Serial<Post>();

            using (var dc = Service.NewSqlContext())
            {
                dc.Execute("INSERT INTO posts () VALUES ()",
                    p =>
                    {
                        p.Set("@authorid", tok.Key);
                        p.Set("@author", tok.Name);
                    }
                );
            }

        }

        public void Remove(WebContext wc)
        {
        }
    }
}