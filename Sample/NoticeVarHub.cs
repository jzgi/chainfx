using Greatbone.Core;

namespace Greatbone.Sample
{
    public class NoticeVarHub : WebVarHub
    {
        public NoticeVarHub(ISetting setg) : base(setg)
        {
        }

        public override void @default(WebContext wc, string id)
        {
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT * FROM notices WHERE id = @1", p => p.Put(id)))
                {
                    Notice obj = dc.GetObj<Notice>();
                    wc.Respond(200, obj);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        public void del(WebContext wc, string id)
        {
            string userid = wc.Token.Key;

            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM notices WHERE id = @1 AND authorid = @2", p => p.Put(id).Put(userid)) > 0)
                {

                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// 
        /// POST /notice/_id_/ask
        /// 
        public void apply(WebContext wc, string var)
        {
            string userid = wc.Token.Key;
            Apply ask = new Apply()
            {
                userid = userid
            };

            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT asks FROM notices WHERE id = @1", p => p.Put(userid)))
                {
                    Apply[] asks = dc.GetArr<Apply>().Concat(ask);

                    if (dc.Execute("UPDATE notices SET asks = @1", p => p.Put(asks)) > 0)
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

        ///
        /// POST /notice/_id_/cmt
        /// {
        ///     "text" : "comment text"            
        /// }            
        public void cmt(WebContext wc, string var)
        {
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
                if (dc.QueryA("SELECT comments FROM notices WHERE id = @1", p => p.Put(var)))
                {
                    Comment[] arr = dc.GetArr<Comment>().Concat(c);
                    if (dc.Execute("UPDATE notices SET WHERE id = @1", p => p.Put(var).Put(tok.Key)) > 0)
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
        public void share(WebContext wc, string var)
        {
        }
    }
}