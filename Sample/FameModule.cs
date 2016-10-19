using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// /fame/
    public class FameModule : WebModule, IAdmin
    {
        public FameModule(WebArg arg) : base(arg)
        {
            SetVarHub<FameVarHub>(false);
        }

        /// <summary>
        /// Get the nth page on top.
        /// </summary>
        /// <code>
        /// GET /fame/top?[page=_n_]
        /// </code>
        public void top(WebContext wc)
        {
            int page = 0;
            wc.Got(nameof(page), ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames ORDER BY rating LIMIT 20 OFFSET @1", p => p.Put(page * 20)))
                {
                    Fame[] fames = dc.ToArr<Fame>();
                    wc.Out(200, fames);
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }
        }

        ///
        /// <summary>
        /// Find records by name matching, or by career.
        /// </summary>
        /// <code> 
        /// GET /fame/find?name=_name_pattern_ OR
        /// GET /fame/find?career=_career_
        /// </code>
        ///
        public void find(WebContext wc)
        {
            string name = null;
            if (wc.Got(nameof(name), ref name))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM fames WHERE name LIKE '%" + name + "%'", null))
                    {
                        Fame[] fames = dc.ToArr<Fame>();
                        wc.Out(200, fames);
                    }
                    else
                    {
                        wc.StatusCode = 204;
                    }
                }
                return;
            }

            string career = null;
            if (wc.Got(nameof(career), ref career))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM fames WHERE careers @> @1", p => p.Put(career)))
                    {
                        Fame[] fames = dc.ToArr<Fame>();
                        wc.Out(200, fames);
                    }
                    else
                    {
                        wc.StatusCode = 204;
                    }
                }
                return;
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
                if (dc.Query("SELECT * FROM fames ORDER BY rating LIMIT 20 OFFSET @1", p => p.Put(name)))
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

            JObj jo = wc.JObj;
            int status = jo[nameof(status)];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE fames SET status = @1 WHERE id = @2", p => p.Put(status));
            }
        }
    }
}