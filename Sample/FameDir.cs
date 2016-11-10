using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    ///
    /// /fame/
    ///
    public class FameDir : WebDir
    {
        public FameDir(WebDirContext ctx) : base(ctx)
        {
            SetVariable<FameVariableDir>();
        }

        public void @default(int page, WebContext wc)
        {
            top(page, wc);
        }

        ///
        /// Get the nth page on top.
        ///
        /// <code>
        /// GET /fame/top[-_page_]
        /// </code>
        public void top(int page, WebContext wc)
        {
            using (var dc = Service.NewDbContext())
            {
                const byte z = 0xff ^ BIN;
                DbSql sql =
                    new DbSql("SELECT ").columnlst(new Fame(), z)._("FROM fames ORDER BY rating LIMIT 20 OFFSET @1");
                if (dc.Query(sql.ToString(), p => p.Put(page * 20)))
                {
                    Fame[] fames = dc.ToDatas<Fame>(z);
                    wc.SendJson(200, fames, z);
                }
                else
                    wc.StatusCode = 204;
            }
        }

        ///
        /// Find records by name matching, or by career.
        ///
        /// <code> 
        /// GET /fame/find?name=_name_pattern_ OR
        /// GET /fame/find?skill=_skill_
        /// </code>
        ///
        public void find(WebContext wc)
        {
            const byte z = 0xff ^ BIN;
            string name = wc[nameof(name)];
            if (name != null)
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(Fame.Empty, z)._("FROM fames WHERE name LIKE '%" + name + "%'");
                    if (dc.Query(sql.ToString()))
                    {
                        var fames = dc.ToDatas<Fame>(z);
                        wc.SendJson(200, fames, z);
                    }
                    else
                        wc.StatusCode = 204;
                }
                return;
            }

            string skill = wc[nameof(skill)];
            if (skill != null)
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(Fame.Empty, z)._("FROM fames WHERE @1 = ANY (skills)");
                    if (dc.Query(sql.ToString(), p => p.Put(skill)))
                    {
                        var fames = dc.ToDatas<Fame>(z);
                        wc.SendJson(200, fames, z);
                    }
                    else
                        wc.StatusCode = 204;
                }
            }
        }

        //
        // ADMIN
        //

        public void srch(WebContext wc)
        {
            int id = wc[nameof(id)];
            string name = wc[nameof(name)];

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
            int id = wc[nameof(id)];
            using (var dc = Service.NewDbContext())
            {
                dc.Execute("DELETE fames WHERE id = @2", p => p.Put(id));
            }
        }

        public void status(WebContext wc, string subscpt)
        {
            int id = wc[nameof(id)];
            Obj obj = wc.ReadObj();
            int status = obj[nameof(status)];

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