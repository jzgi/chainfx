using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{

    ///
    /// /fame/
    public class FameWork : WebWork, IMgmt
    {
        public FameWork(WebWorkContext wwwc) : base(wwwc)
        {
            SetVar<FameMuxWork>();
        }

        public override void @default(WebContext wc, string subscpt)
        {
            top(wc, subscpt);
        }



        /// <summary>
        /// Get the nth page on top.
        /// </summary>
        /// <code>
        /// GET /fame/top[-_n_]
        /// </code>
        public void top(WebContext wc, string subscpt)
        {
            string id = wc[0];
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                const byte z = 0xff ^ BIN;
                DbSql sql = new DbSql("SELECT ").columnlst(new Fame(), z)._("FROM fames ORDER BY rating LIMIT 20 OFFSET @1");
                if (dc.Query(sql.ToString(), p => p.Put(n * 20)))
                {
                    Fame[] fames = dc.ToBeans<Fame>(z);
                    wc.SendJson(200, fames, z);
                }
                else
                    wc.StatusCode = 204;
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
            const byte z = 0xff ^ BIN;
            string name = null;
            if (wc.Get(nameof(name), ref name))
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(Fame.Empty, z)._("FROM fames WHERE name LIKE '%" + name + "%'");
                    if (dc.Query(sql.ToString()))
                    {
                        Fame[] fames = dc.ToBeans<Fame>(z);
                        wc.SendJson(200, fames, z);
                    }
                    else
                        wc.StatusCode = 204;
                }
                return;
            }

            string skill = null;
            if (wc.Get(nameof(skill), ref skill))
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(Fame.Empty, z)._("FROM fames WHERE @1 = ANY (skills)");
                    if (dc.Query(sql.ToString(), p => p.Put(skill)))
                    {
                        Fame[] fames = dc.ToBeans<Fame>(z);
                        wc.SendJson(200, fames, z);
                    }
                    else
                        wc.StatusCode = 204;
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

            Obj jo = wc.ReadObj();
            int status = jo[nameof(status)];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE fames SET status = @1 WHERE id = @2", p => p.Put(status));
            }
        }

        public void mgmt(WebContext wc, string subscpt)
        {
            throw new NotImplementedException();
        }
    }
}