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

            mgmtWas = Actions(nameof(srch), nameof(del), nameof(status));
        }

        public override void @default(WebContext wc, string subscpt)
        {
            top(wc, subscpt);
        }

        /// <summary>
        /// Get the nth page of records on top.
        /// </summary>
        /// <code>
        /// GET /post/top-[_n_][?authorid=_id_]
        /// </code>
        public void top(WebContext wc, string subscpt)
        {
            int page = subscpt.ToInt();
            string authorid = null;
            const byte x = 0xff ^ BIN;
            if (wc.Get(nameof(authorid), ref authorid))
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, x)._("FROM posts WHERE authorid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2");
                    if (dc.Query(sql.ToString(), p => p.Put(authorid).Put(20 * page)))
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
            else
            {
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
            JObj jo = wc.ReadJObj();
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
        [CheckAdmin]
        [Button(IsGet = true, Icon = FaUtility.chrome)]
        public override void mgmt(WebContext wc, string subscpt)
        {
            // returh first UI
            wc.SendMajorLayout(200, "管理功能", a =>
            {
                a.form(mgmtWas, (Post[])null, 0);
            });
        }

        [CheckAdmin]
        [Button(IsGet = true, Icon = FaUtility.chrome, Dialog = 3)]
        public void srch(WebContext wc, string subscpt)
        {
            using (var dc = Service.NewDbContext())
            {
                const byte x = 0xff ^ BIN;
                DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, x)._("FROM posts");
                if (dc.Query(sql.ToString()))
                {
                    Post[] arr = dc.ToArr<Post>(x);
                    wc.SendMajorLayout(200, "管理功能", a =>
                    {
                        a.form(mgmtWas, arr, x);
                    });
                }
                else
                {
                    wc.StatusCode = 204; // no content
                }
            }
        }

        [Button]
        public void del(WebContext wc, string subscpt)
        {
            if (wc.IsGetMethod) // return confirmation dialog
            {

            }
            else //
            {

            }
        }

        [Button]
        public void status(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

    }

}