using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The notice service.</summary>
    ///
    public class NoticeModule : WebModule, IAdmin
    {
        public NoticeModule(ISetting setg) : base(setg)
        {
            SetVarHub<NoticeVarHub>(false);
        }

        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void @default(WebContext wc)
        {
            int page = 0;
            wc.Get("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM notices WHERE duedate <= current_date ORDER BY id LIMIT 20 OFFSET @offset", p => p.Put("@offset", page * 20)))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }


        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        public void @new(WebContext wc)
        {
            JObj o = (JObj)wc.Data;
            int age = o[nameof(age)];

            int page = 0;
            wc.Get("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("INSERT INTO notices () VALUES ()",
                    p => { p.Put(page * 20); p.Put(page * 20); }))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }

        public void search(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void status(WebContext wc)
        {
            throw new NotImplementedException();
        }

        public void del(WebContext wc)
        {
            throw new NotImplementedException();
        }
    }
}