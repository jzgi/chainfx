using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class PostMultiple : WebMultiple
    {
        public PostMultiple(WebArg arg) : base(arg)
        {
        }

        ///
        /// <code>
        /// GET /post/_id_/
        /// </code>
        ///
        public override void @default(WebContext wc, string subscpt)
        {
        }

        /// <summary>
        /// Get the nth image.
        /// </summary>
        /// <code>
        /// GET /post/_id_/img[-_n_]
        /// </code>
        ///
        public void img(WebContext wc, string subscpt)
        {
            int id = wc.Super.Int();
            int n = subscpt.Int();
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT m" + n + " FROM posts WHERE id = @1", p => p.Put(id)))
                {
                    byte[] v = dc.GotBytes();
                    StaticContent sta = new StaticContent() { Buffer = v };
                    wc.Out(200, sta, true, 60000);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        /// <summary>
        /// Update the nth image.
        /// </summary>
        /// <code>
        /// POST /post/_id_/updimg-[_n_]
        /// [img_bytes]
        /// </code>
        ///
        public void updimg(WebContext wc, string subscpt)
        {
            int id = wc.Super.Int();
            int n = subscpt.Int();
            using (var dc = Service.NewDbContext())
            {
                ArraySegment<byte>? bytes = wc.BytesSeg;
                if (bytes == null)
                {
                    wc.StatusCode = 301; ;
                }
                else if (dc.Execute("UPDATE posts SET m" + n + " = @1 WHERE id = @2", p => p.Put(bytes.Value).Put(id)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// <code>
        /// POST /post/_id_/del
        /// </code>
        ///
        public void del(WebContext wc, string subscpt)
        {
            IToken tok = wc.Token;
            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute("DELETE FROM posts WHERE id = @1 AND authorid = @2", p => p.Put(subscpt).Put(tok.Key)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }


        ///
        /// <code>
        /// POST /post/_id_/cmt
        /// {
        ///     "text" : "comment text"            
        /// }            
        /// </code>
        ///
        public void cmt(WebContext wc, string subscpt)
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
                if (dc.QueryA("SELECT comments FROM posts WHERE id = @1", p => p.Put(subscpt)))
                {
                    Comment[] arr = dc.GotArr<Comment>().Add(c);

                    // add new

                    if (dc.Execute("UPDATE posts SET WHERE id = @1", p => p.Put(subscpt).Put(tok.Key)) > 0)
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
        /// <code>
        /// POST /post/_id_/share
        /// </code>
        ///
        public void share(WebContext wc, string subscpt)
        {
        }
    }
}