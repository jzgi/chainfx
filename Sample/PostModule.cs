using System;
using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{

    public class PostModule : AbstModule, IMgmt
    {
        readonly WebAction[] mgmtWas;

        public PostModule(WebArg arg) : base(arg)
        {
            SetMultiple<PostMultiple>();

            mgmtWas = Actions(nameof(mgmt), nameof(srch), nameof(del), nameof(status));
        }

        public override void @default(WebContext wc, string subscpt)
        {
            top(wc, subscpt);
        }

        /// <summary>
        /// Get the nth page of records on top.
        /// </summary>
        /// <code>
        /// GET /post/top-[_n_]
        /// </code>
        public void top(WebContext wc, string subscpt)
        {
            const byte x = 0xff ^ BIN;

            int page = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, x)._("FROM posts ORDER BY id DESC LIMIT 20 OFFSET @1");
                if (dc.Query(sql.ToString(), p => p.Put(20 * page)))
                {
                    Post[] arr = dc.ToArr<Post>(x);
                    wc.SendJ(200, arr, x);
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
            IPrincipal tok = wc.Principal;
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
        [ToAdmin]
        public override void mgmt(WebContext wc, string subscpt)
        {
            // returh first UI
            wc.SendMajorLayout(200, "管理功能", a =>
            {
                // a.Form(,,,,, )
                a.buttonlst(mgmtWas);
            });

        }

        [Dialog]
        public void srch(WebContext wc, string subscpt)
        {
            Form frm = wc.Form;

            string word = null;
            if (wc.Got(nameof(word), ref word))
            {

            }
            else
            { // return search condition dialog 

            }
        }

        [Dialog]
        public void del(WebContext wc, string subscpt)
        {
            if (wc.IsGet) // return confirmation dialog
            {

            }
            else //
            {

            }
        }

        [Dialog]
        public void status(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

    }

}