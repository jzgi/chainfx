using Greatbone.Core;
using System;

namespace Greatbone.Sample
{
    public class PostModule : WebModule, IAdmin
    {
        public PostModule(WebArg arg) : base(arg)
        {
            SetMultiple<PostMultiple>(false);
        }


        static string topsql = new DbSql("SELECT ").columnlst(new Post(), XUtility.NoBinary)._("FROM posts ORDER BY id DESC LIMIT 20 OFFSET @1").ToString();

        /// <summary>
        /// Get the nth page of records on top.
        /// </summary>
        /// <code>
        /// GET /post/top-[_n_]
        /// </code>
        public void top(WebContext wc, string subscpt)
        {
            int page = subscpt.Int();
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query(topsql, p => p.Put(20 * page)))
                {
                    Post[] arr = dc.ToArr<Post>(XUtility.NoBinary);
                    wc.Out(200, arr);
                }
                else
                {
                    wc.StatusCode = 204; // no content
                }
            }
        }

        /// <summary>
        /// Create a new record.
        /// </summary>
        /// <code>
        /// POST /post/new
        /// {
        ///     "commentable" : _true_or_false_ 
        ///     "text" : "_text_content_" 
        /// }
        /// </code>
        ///
        public void @new(WebContext wc, string subscpt)
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
                wc.SetHeader("Location", id.ToString());
                wc.StatusCode = 201;
            }
        }


        //
        // ADMIN
        //

        public void search(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void del(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

    }
}