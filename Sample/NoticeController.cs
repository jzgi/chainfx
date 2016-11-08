using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{

    public class NoticeController : AbstController, IMgmt
    {
        public NoticeController(WebArg arg) : base(arg)
        {
            SetMuxer<NoticeMuxer>();
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
            if (wc.Get(nameof(authorid), ref authorid))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM notices WHERE authorid = @1 ORDER BY id LIMIT 20 OFFSET @2", p => p.Put(authorid).Put(page * 20)))
                    {
                        Notice[] arr = dc.ToArr<Notice>(0xff);
                        wc.SendJ(200, arr, 0xff);
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
                        Notice[] arr = dc.ToArr<Notice>(0xff);
                        wc.SendJ(200, arr, 0xff);
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
        ///     "loc" : "",            
        ///     "duedate" : "2016-12-12",            
        ///     "subject" : "_subject_",
        ///     "tel" : "_telephone_", 
        ///     "text" : "_content_", 
        ///     "commentable" : true
        /// }
        /// </code>
        ///
        [Check]
        public void @new(WebContext wc, string subscpt)
        {
            IPrincipal tok = wc.Principal;
            Notice obj = wc.ReadObj<Notice>();

            obj.authorid = tok.Key;
            obj.author = tok.Name;
            obj.date = DateTime.Now;
            using (var dc = Service.NewDbContext())
            {
                const byte z = 0xff ^ AUTO;
                DbSql sql = new DbSql("INSERT INTO notices")._(Notice.Empty, z)._VALUES_(Notice.Empty, z)._("RETURNING id");
                object id = dc.Scalar(sql.ToString(), p => obj.Dump(p, z));
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