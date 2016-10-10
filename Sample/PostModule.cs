using Greatbone.Core;
using System;

namespace Greatbone.Sample
{
    ///
    /// /post/
    public class PostModule : WebModule, IAdmin
    {
        public PostModule(ISetting setg) : base(setg)
        {
            SetVarHub<PostVarHub>(false);
        }

        ///
        /// GET /post/top?[page=_num_]
        ///
        public void top(WebContext wc)
        {
            int page = 0;
            wc.Got(nameof(page), ref page);
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query(@"SELECT * FROM posts ORDER BY id DESC LIMIT @limit OFFSET @offset", p => p.Put(20).Put(20 * page)))
                {
                    Post[] posts = null;
                    dc.Got(ref posts);
                    wc.SendArr(200, posts);
                }
                else
                {
                    wc.StatusCode = 204; // no content
                }
            }
        }

        ///
        /// POST /post/new
        public void @new(WebContext wc)
        {
            IToken tok = wc.Token;

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("INSERT INTO posts () VALUES ()", p => p.Put(tok.Key).Put(tok.Name));
            }
        }


        //
        // ADMIN
        //

        public void search(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void del(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc)
        {
            throw new NotImplementedException();
        }

    }
}