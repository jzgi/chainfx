using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class PostVarHub : WebVarHub
    {
        public PostVarHub(ISetting setg) : base(setg)
        {
        }

        ///
        /// <code>
        /// GET /post/_id_/
        /// </code>
        ///
        public override void @default(WebContext wc, string var)
        {
        }

        ///
        /// <code>
        /// GET /post/_id_/img/idx=_idx_
        /// </code>
        ///
        /// <code>
        /// POST /post/_id_/img/idx=_idx_
        /// ......
        /// </code>
        ///
        public void img(WebContext wc, string var)
        {
            int idx = 0;
            if (wc.Got(nameof(idx), ref idx))
            {
                if (wc.IsGet)
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.QueryA("SELECT m" + idx + " FROM fames WHERE id = @1", p => p.Put(var)))
                        {
                            byte[] v = dc.GetBytes();
                            StaticContent sta = new StaticContent()
                            {
                                Buffer = v
                            };
                            wc.Respond(200, sta, true, 60000);
                        }
                        else
                        {
                            wc.StatusCode = 404;
                        }
                    }
                }
                else
                {
                    using (var dc = Service.NewDbContext())
                    {
                        ArraySegment<byte>? bytes = wc.BytesSeg;
                        if (bytes == null)
                        {
                            wc.StatusCode = 304; ;
                        }
                        else if (dc.Execute("UPDATE posts SET m" + idx + " = @1 WHERE id = @2", p => p.Put(bytes.Value).Put(var)) > 0)
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
            else
            {
                wc.StatusCode = 304;
            }
        }

        ///
        /// <code>
        /// POST /post/_id_/del
        /// </code>
        ///
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


        ///
        /// <code>
        /// POST /post/_id_/cmt
        /// {
        ///     "text" : "comment text"            
        /// }            
        /// </code>
        ///
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
                    Comment[] arr = dc.GetArr<Comment>().Add(c);

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
        /// <code>
        /// POST /post/_id_/share
        /// </code>
        ///
        public void share(WebContext wc, string var)
        {
        }
    }
}