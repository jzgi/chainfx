using Greatbone.Core;
using System;

namespace Greatbone.Sample
{
    public class PostModule : WebModule, IAdmin
    {
        public PostModule(WebArg arg) : base(arg)
        {
            SetMultiple<PostMultiple>();
        }


        static string TopSql = new DbSql("SELECT ").columnlst(new Post())._("FROM posts ORDER BY id DESC LIMIT 20 OFFSET @1").ToString();

        /// <summary>
        /// Get the nth page of records on top.
        /// </summary>
        /// <code>
        /// GET /post/top-[_n_]
        /// </code>
        public void top(WebContext wc, string subscpt)
        {
            int page = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query(TopSql, p => p.Put(20 * page)))
                {
                    Post[] arr = dc.ToArr<Post>();
                    wc.OutJ(200, arr);
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
        public override void @default(WebContext wc, string subscpt)
        {
            WebInterface iadm = GetInterface(typeof(IAdmin));

            // returh first UI
            wc.OutPrimeHt(200, "管理功能", a =>
            {
                a.BUTTONS(iadm);
            });

        }

        [Button(ShowDialog = "srchdlg")]
        public void srch(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        [Dialog(3)]
        public void srchdlg(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        [Button(ShowDialog = ""), Dialog(2)]
        public void del(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        [Button(ShowDialog = ""), Dialog(2)]
        public void status(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

    }
}