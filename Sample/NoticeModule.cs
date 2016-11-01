using System;
using Greatbone.Core;
using static Greatbone.Core.XUtility;

namespace Greatbone.Sample
{

    public class NoticeModule : AbstModule, IMgmt
    {
        public NoticeModule(WebArg arg) : base(arg)
        {
            SetMultiple<NoticeMultiple>();
        }

        public override void @default(WebContext wc, string subscpt)
        {
            top(wc, subscpt);
        }

        ///
        /// <code>
        /// GET /notice/top[-_n_][?authorid=_id_]
        /// </code>
        ///
        public void top(WebContext wc, string subscpt)
        {
            int page = subscpt.ToInt();
            string authorid = null;
            if (wc.Got(nameof(authorid), ref authorid))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM notices WHERE authorid = @1 ORDER BY id LIMIT 20 OFFSET @2", p => p.Put(authorid).Put(page * 20)))
                    {
                        Notice[] arr = dc.ToArr<Notice>();
                        wc.SendJ(200, arr);
                    }
                    else
                    {
                        wc.StatusCode = 204;
                    }
                }
            }
            else
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM notices WHERE duedate >= current_date ORDER BY id LIMIT 20 OFFSET @1", p => p.Put(page * 20)))
                    {
                        Notice[] arr = dc.ToArr<Notice>();
                        wc.SendJ(200, arr);
                    }
                    else
                    {
                        wc.StatusCode = 204;
                    }
                }
            }
        }

        ///
        /// <code>
        /// POST /post/new
        /// {
        ///     "commentable" : true 
        ///     "text" : "text content" 
        /// }
        /// </code>
        ///
        public void @new(WebContext wc, string subscpt)
        {
            const byte x = 0xff ^ AUTO;

            IPrincipal tok = wc.Principal;
            Notice obj = wc.Obj<Notice>();
            obj.authorid = tok.Key;
            obj.author = tok.Name;
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("INSERT INTO notices")._(Notice.Empty, x)._VALUES_(Notice.Empty, x)._("RETURNING id");
                object id = dc.Scalar(sql.ToString(), p => obj.Save(p, x));
                if (id != null)
                {
                    wc.StatusCode = 201;
                    wc.SetHeader("Location", id.ToString());
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }
        }

        public void srch(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

        public void del(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }

    }
}