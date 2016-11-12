using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    public class PostGrpDir : WebDir
    {
        public PostGrpDir(WebDirContext ctx) : base(ctx)
        {
            SetMux<PostGrpMuxDir>();
        }

        public void @default(WebContext wc, int page)
        {
            top(wc, page);
        }

        ///
        /// Get the nth page of groups by specified categpry.
        ///
        /// <code>
        /// GET /post/top-[_page_][?cat=_catepgry_]
        /// </code>
        public void top(WebContext wc, int page)
        {
            const byte z = 0xff ^ BIN;

            string cat = wc[nameof(cat)];
            if (cat != null)
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(PostGrp.Empty, z)._("FROM postgrps WHERE cat = @1 ORDER BY rating DESC LIMIT 20 OFFSET @2");
                    if (dc.Query(sql.ToString(), p => p.Put(cat).Put(20 * page)))
                    {
                        var postgrps = dc.ToDatas<PostGrp>(z);
                        wc.SendJson(200, postgrps, z);
                    }
                    else
                        wc.StatusCode = 204; // no content
                }
            }
            else
            {
                using (var dc = Service.NewDbContext())
                {
                    DbSql sql = new DbSql("SELECT ").columnlst(PostGrp.Empty, z)._("FROM postgrps ORDER BY rating DESC LIMIT 20 OFFSET @1");
                    if (dc.Query(sql.ToString(), p => p.Put(20 * page)))
                    {
                        var postgrps = dc.ToDatas<PostGrp>(z);
                        wc.SendJson(200, postgrps, z);
                    }
                    else
                        wc.StatusCode = 204; // no content
                }
            }
        }
    }
}