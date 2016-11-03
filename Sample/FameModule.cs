using Greatbone.Core;
using static Greatbone.Core.XUtil;

namespace Greatbone.Sample
{

    ///
    /// /fame/
    public class FameModule : AbstModule, IMgmt
    {
        public FameModule(WebArg arg) : base(arg)
        {
            SetMultiple<FameMultiple>();
        }

        public override void @default(WebContext wc, string subscpt)
        {
            top(wc, subscpt);
        }


        const byte TopX = 0xff ^ BIN;
        static string TopSql = new DbSql("SELECT ").columnlst(new Fame(), TopX)._("FROM fames ORDER BY rating LIMIT 20 OFFSET @1").ToString();

        /// <summary>
        /// Get the nth page on top.
        /// </summary>
        /// <code>
        /// GET /fame/top[-_n_]
        /// </code>
        public void top(WebContext wc, string subscpt)
        {
            string id = wc.Super;
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query(TopSql, p => p.Put(n * 20)))
                {
                    Fame[] fames = dc.ToArr<Fame>(TopX);
                    wc.SendJ(200, fames, TopX);
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
        /// GET /fame/find?skill=_skill_
        /// </code>
        ///
        public void find(WebContext wc, string subscpt)
        {
            string name = null;
            if (wc.Get(nameof(name), ref name))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM fames WHERE name LIKE '%" + name + "%'", null))
                    {
                        Fame[] fames = dc.ToArr<Fame>(0xff ^ BIN);
                        wc.SendJ(200, fames);
                    }
                    else
                    {
                        wc.StatusCode = 204;
                    }
                }
                return;
            }

            string skill = null;
            if (wc.Get(nameof(skill), ref skill))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query("SELECT * FROM fames WHERE @1 = ANY (skills)", p => p.Put(skill)))
                    {
                        Fame[] fames = dc.ToArr<Fame>(0xff ^ BIN);
                        wc.SendJ(200, fames);
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

        public void srch(WebContext wc, string subscpt)
        {
            int id = 0;
            wc.Get(nameof(id), ref id);

            string name = null;
            wc.Get(nameof(name), ref name);

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

        public void del(WebContext wc, string subscpt)
        {
            int id = 0;
            wc.Get(nameof(id), ref id);

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("DELETE fames WHERE id = @2", p => p.Put(id));
            }
        }

        public void status(WebContext wc, string subscpt)
        {
            int id = 0;
            wc.Get(nameof(id), ref id);

            JObj jo = wc.ReadJObj();
            int status = jo[nameof(status)];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE fames SET status = @1 WHERE id = @2", p => p.Put(status));
            }
        }
    }
}