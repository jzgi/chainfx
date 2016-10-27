using Greatbone.Core;

namespace Greatbone.Sample
{
    public class NoticeMultiple : WebMultiple
    {
        public NoticeMultiple(WebArg arg) : base(arg)
        {
        }

        /// <summary>
        /// Get the record.
        /// </summary>
        /// <code>
        /// GET /notice/_id_/
        /// </code>
        /// 
        public override void @default(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT * FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    Notice obj = dc.ToObj<Notice>();
                    wc.SendJ(200, obj);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Delete the record.
        /// </summary>
        /// <code>
        /// POST /notice/_id_/del
        /// </code>
        /// 
        public void del(WebContext wc, string subscpt)
        {
            int id = subscpt.ToInt();
            string userid = wc.Token.Key;
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM notices WHERE id = @1 AND authorid = @2", p => p.Put(id).Put(userid)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Apply for this opportunity.
        /// </summary>
        /// <code>
        /// POST /notice/_id_/apply
        /// </code>
        /// 
        public void apply(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            string userid = wc.Token.Key;
            App app = new App()
            {
                userid = userid
            };
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT apps FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    App[] arr = dc.GotArr<App>().Add(app);
                    if (dc.Execute("UPDATE notices SET apps = @1", p => p.Put(arr)) > 0)
                    {
                        wc.StatusCode = 201;
                    }
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Add a comment to this notice.
        /// </summary>
        /// <code>
        /// POST /notice/_id_/cmt
        /// {
        ///     "text" : "comment text"            
        /// }            
        /// </code>
        public void cmt(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            IToken tok = wc.Token;
            JObj jo = wc.JObj;
            string text = jo[nameof(text)];

            Comment c = new Comment
            {
                authorid = tok.Key,
                text = text
            };

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT comments FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    Comment[] arr = dc.GotArr<Comment>().Add(c);
                    if (dc.Execute("UPDATE notices SET comments = @1 WHERE id = @2", p => p.Put(arr).Put(id)) > 0)
                    {
                        wc.StatusCode = 200;
                    }
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }

        }

        ///
        /// POST /notice/_id_/share
        public void share(WebContext wc, string subscpt)
        {
            int id = wc.Super.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("UPDATE notices SET shared = shared + 1 WHERE id = @1", p => p.Put(id)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

    }
}