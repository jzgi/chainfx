using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public class NoticeModule : WebModule, IMgmt
    {
        public NoticeModule(WebArg arg) : base(arg)
        {
            SetMultiple<NoticeMultiple>();
        }

        /// <code>
        /// GET /notice/top?[page=_num_]
        /// </code>
        public void top(WebContext wc, string subscpt)
        {
            int page = 0;
            wc.Got(nameof(page), ref page);
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM notices WHERE duedate <= current_date ORDER BY id LIMIT 20 OFFSET @1", p => p.Put(page * 20)))
                {
                    Notice[] arr = dc.ToArr<Notice>();
                    wc.OutJ(200, arr);
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }
        }


        string NewSql = new DbSql("INSERT INTO notices")._(new Notice())._VALUES_(new Notice())._("RETURNING id").ToString();

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
            IToken tok = wc.Token;
            Notice obj = wc.JObj.ToObj<Notice>();
            obj.authorid = tok.Key;
            obj.author = tok.Name;
            using (var dc = Service.NewDbContext())
            {
                object id = dc.Scalar(NewSql.ToString(), p => obj.Save(p));
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