using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class NoticeVariableDir : WebDir, IVariable
    {
        public NoticeVariableDir(WebDirContext wnc) : base(wnc)
        {
        }

        ///
        /// Get the record.
        ///
        /// <code>
        /// GET /notice/_id_/
        /// </code>
        /// 
        public void @default(WebContext wc, string subscpt)
        {
            int id = wc.Var(this);
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT * FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    Notice obj = dc.ToData<Notice>();
                    wc.SendJson(200, obj);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// Delete the record.
        ///
        /// <code>
        /// POST /notice/_id_/del
        /// </code>
        /// 
        [Check]
        public void del(WebContext wc, string subscpt)
        {
            int id = subscpt.ToInt();
            string userid = wc.Principal.Key;
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM notices WHERE id = @1 AND authorid = @2", p => p.Put(id).Put(userid)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// Apply for this opportunity.
        ///
        /// <code>
        /// POST /notice/_id_/apply
        /// </code>
        /// 
        [Check]
        public void apply(WebContext wc, string subscpt)
        {
            int id = wc.Var(this);
            string userid = wc.Principal.Key;
            App app = new App()
            {
                userid = userid
            };
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT apps FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    App[] arr = dc.GetDatas<App>().Add(app);
                    if (dc.Execute("UPDATE notices SET apps = @1", p => p.Put(arr)) > 0)
                    {
                        wc.StatusCode = 201;
                    }
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// Add a comment to this notice.
        /// 
        /// <code>
        /// POST /notice/_id_/cmt
        /// {
        ///     "text" : "comment text"            
        /// }            
        /// </code>
        [Check]
        public void cmt(WebContext wc, string subscpt)
        {
            int id = wc.Var(this);
            IPrincipal tok = wc.Principal;
            Comment c = wc.ReadData<Comment>();

            c.time = DateTime.Now;
            c.authorid = tok.Key;

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT comments FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    Comment[] cmts = dc.GetDatas<Comment>().Add(c);
                    if (dc.Execute("UPDATE notices SET comments = @1 WHERE id = @2", p => p.Put(cmts).Put(id)) > 0)
                    {
                        wc.StatusCode = 200;
                    }
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// POST /notice/_id_/share
        public void share(WebContext wc, string subscpt)
        {
            int id = wc.Var(this);
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("UPDATE notices SET shared = shared + 1 WHERE id = @1", p => p.Put(id)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }
    }
}