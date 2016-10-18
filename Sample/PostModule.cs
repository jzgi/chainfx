using Greatbone.Core;
using System;

namespace Greatbone.Sample
{
    public class PostModule : WebModule, IAdmin
    {
        public PostModule(WebArg arg) : base(arg)
        {
            SetVarHub<PostVarHub>(false);
        }

        /// <code>
        /// GET /post/top?[page=_num_]
        /// </code>
        public void top(WebContext wc)
        {
            int page = 0;
            wc.Got(nameof(page), ref page);
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query(@"SELECT * FROM posts ORDER BY id DESC LIMIT @limit OFFSET @offset", p => p.Put(20).Put(20 * page)))
                {
                    Post[] arr = dc.ToArr<Post>();
                    wc.Respond(200, arr);
                }
                else
                {
                    wc.StatusCode = 204; // no content
                }
            }
        }

        ///
        /// <code>
        /// POST /post/new
        ///
        /// {
        ///     "commentable" : true 
        ///     "text" : "text content" 
        /// }
        /// </code>
        ///
        public void @new(WebContext wc)
        {
            IToken tok = wc.Token;
            JObj jo = wc.JObj;
            DateTime time = DateTime.Now;
            bool commentable = jo[nameof(commentable)];
            string text = jo[nameof(text)];

            using (var dc = Service.NewDbContext())
            {
                object id = dc.Scalar("INSERT INTO posts (time, authorid, author, commentable, text) VALUES (@1,@2,@3,@4,@5) RETURNING ID",
                     p => p.Put(time).Put(tok.Key).Put(tok.Name).Put(commentable).Put(text));
                wc.SetLocation(id.ToString());
                wc.StatusCode = 201;
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