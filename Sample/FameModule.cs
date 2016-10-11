using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// /fame/
    public class FameModule : WebModule, IAdmin
    {
        public FameModule(ISetting setg) : base(setg)
        {
            SetVarHub<FameVarHub>(false);
        }

        ///
        /// GET /fame/top?[page=_num_]
        public void top(WebContext wc)
        {
            int page = 0;
            wc.Got(nameof(page), ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY rating LIMIT 20 OFFSET @1", p => p.Put(page * 20)))
                {
                    Fame[] fames = dc.GetArr<Fame>();
                    // dc.Got(ref fames);
                    wc.Respond(200, fames);
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }
        }

        ///
        /// GET /fame/find?word=_name_
        ///
        public void find(WebContext wc)
        {
            string word = null;
            wc.Got(nameof(word), ref word);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE name LIKE '%" + word + "%'", null))
                {
                    Fame[] fames = dc.GetArr<Fame>();
                    // dc.Got(ref fames);
                    wc.Respond(200, fames);
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

            JObj jo = wc.JObj;
            int status = jo[nameof(status)];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE fames SET status = @1 WHERE id = @2", p => p.Put(status));
            }
        }
    }
}