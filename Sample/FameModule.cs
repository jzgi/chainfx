using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FameModule : WebModule, IAdmin
    {
        public FameModule(ISetting setg) : base(setg)
        {
            SetVarHub<FameVarHub>(false);
        }

        /// <summary>
        /// Gets the top list of fames. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void @default(WebContext wc)
        {
            int page = 0;
            wc.Got("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY  LIMIT 20 OFFSET @offset",
                    p => p.Put("@offset", page * 20)))
                {
                    while (dc.NextRow())
                    {
                    }
                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }


        public void top(WebContext wc)
        {
            int page = 0;
            wc.Got("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY rating LIMIT 20 OFFSET @offset",
                    p => p.Put("@offset", page * 20)))
                {
                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }

        //
        // ADMIN
        //

        public void search(WebContext wc)
        {
            int id = 0;
            wc.Got(nameof(id), ref id);

            string name = null;
            wc.Got(nameof(name), ref name);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY rating LIMIT 20 OFFSET @1", p => p.Put(name)))
                {
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }

        }

        public void del(WebContext wc)
        {
            int id = 0;
            wc.Got(nameof(id), ref id);
            
            using (var dc = Service.NewDbContext())
            {
                dc.Execute("DELETE fames WHERE id = @2", p => p.Put(id));
            }
        }

        public void status(WebContext wc)
        {
            int id = 0;
            wc.Got(nameof(id), ref id);

            JObj jo = (JObj)wc.Data;
            int status = jo[nameof(status)];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE fames SET status = @1 WHERE id = @2", p => p.Put(status));
            }
        }
    }
}