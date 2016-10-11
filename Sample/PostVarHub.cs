using Greatbone.Core;

namespace Greatbone.Sample
{
    public class PostVarHub : WebVarHub
    {
        public PostVarHub(ISetting setg) : base(setg)
        {
        }

        ///
        /// GET /post/_id_/
        public override void @default(WebContext wc, string var)
        {
        }

        ///
        /// GET /post/_id_/img/idx=_pic_idx_
        public void img(WebContext wc, string var)
        {
            int idx = 0;
            if (wc.Got(nameof(idx), ref idx))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.QueryA("SELECT m" + idx + " FROM fames WHERE id = @1", p => p.Put(var)))
                    {
                        byte[] v = dc.GetBytes();
                        wc.SendBytes(200, v);
                    }
                    else
                    {
                        wc.StatusCode = 404;
                    }
                }
            }
            else
            {
                wc.StatusCode = 304;
            }
        }

        ///
        /// POST /post/_id_/del
        public void del(WebContext wc, string var)
        {
            IToken tok = wc.Token;
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM posts WHERE id = @1 AND authorid = @2", p => p.Put(var).Put(tok.Key)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// POST /post/_id_/updimg
        ///
        public void updimg(WebContext wc)
        {
            // ArraySegment<byte> bytes = wc.Bytes;
            // using (var dc = Service.NewDbContext())
            // {
            //     dc.Execute("INSERT INTO posts () VALUES ()", p => p.Put(tok.Key).Put(tok.Name));
            // }
        }

        ///
        /// POST /post/_id_/cmt
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
                if (dc.QueryA("SELECT comments FROM posts WHERE id = @1", p => p.Put(var)))
                {
                    Comment[] arr = dc.GetArr<Comment>().Concat(c);

                    // add new

                    if (dc.Execute("UPDATE posts SET WHERE id = @1", p => p.Put(var).Put(tok.Key)) > 0)
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
        /// /POST/_id_/share
        public void share(WebContext wc, string var)
        {
        }
    }
}