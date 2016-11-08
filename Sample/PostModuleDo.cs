using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{

    public class PostModuleDo : AbstModuleDo, IMgmt
    {
        readonly WebAction[] mgmtWas;

        public PostModuleDo(WebArg arg) : base(arg)
        {
            SetMux<PostMuxDo>();

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
            const byte z = 0xff ^ BIN;
            if (wc.Get(nameof(authorid), ref authorid))
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, z)._("FROM posts WHERE authorid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2");
                    if (dc.Query(sql.ToString(), p => p.Put(authorid).Put(20 * page)))
                    {
                        Post[] arr = dc.ToArr<Post>(z);
                        wc.SendJ(200, arr, z);
                    }
                    else
                        wc.StatusCode = 204; // no content
                }
            }
            else
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, z)._("FROM posts ORDER BY id DESC LIMIT 20 OFFSET @1");
                    if (dc.Query(sql.ToString(), p => p.Put(20 * page)))
                    {
                        Post[] arr = dc.ToArr<Post>(z);
                        wc.SendJ(200, arr, z);
                    }
                    else
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
        [Check]
        public void @new(WebContext wc, string subscpt)
        {
            IPrincipal tok = wc.Principal;
            Post obj = wc.ReadObj<Post>();
            obj.time = DateTime.Now;
            obj.authorid = tok.Key;
            obj.author = tok.Name;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("INSERT INTO posts")._(Post.Empty)._VALUES_(Post.Empty)._(" RETURNING ID");
                object id = dc.Scalar(sql.ToString(), p => obj.Dump(p));
                if (id != null)
                {
                    wc.SetHeader("Location", id.ToString());
                    wc.StatusCode = 201;
                }
                else
                    wc.StatusCode = 300;
            }
        }


        //
        // ADMIN
        //
        [CheckAdmin]
        [Button(IsGet = true, Icon = "fa fa-chrome")]
        public override void mgmt(WebContext wc, string subscpt)
        {
            // returh first UI
            wc.SendMajorLayout(200, "管理功能", a =>
            {
                a.form(mgmtWas, (Post[])null, 0);
            });
        }

        [CheckAdmin]
        [Button(IsGet = true, Icon = "fa fa-chrome", Dialog = 3)]
        public void srch(WebContext wc, string subscpt)
        {
            using (var dc = Service.NewDbContext())
            {
                const byte z = 0xff ^ BIN;
                DbSql sql = new DbSql("SELECT ").columnlst(Post.Empty, z)._("FROM posts");
                if (dc.Query(sql.ToString()))
                {
                    Post[] arr = dc.ToArr<Post>(z);
                    wc.SendMajorLayout(200, "管理功能", a =>
                    {
                        a.form(mgmtWas, arr, z);
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
                wc.SendDialogLayout(200, a => a.T("delete the selected items?"));
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