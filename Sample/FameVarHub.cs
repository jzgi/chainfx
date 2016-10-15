using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /fame/_id_/
    ///
    public class FameVarHub : WebVarHub
    {
        public FameVarHub(ISetting setg) : base(setg)
        {
        }

        ///
        /// GET /fame/_id_/
        public override void @default(WebContext wc, string var)
        {
            throw new System.NotImplementedException();
        }

        public void upd(WebContext wc, string var)
        {
            throw new System.NotImplementedException();
        }

        ///
        /// <code>
        /// GET /fame/_id_/icon
        /// </code>
        ///
        /// <code>
        /// POST /fame/_id_/icon
        /// .....
        /// </code>
        ///
        public void icon(WebContext wc, string var)
        {
            string id = var;
            using (var dc = Service.NewDbContext())
            {
                if (wc.IsGet)
                {
                    if (dc.QueryA("SELECT icon FROM fames WHERE id = @1", p => p.Put(id)))
                    {
                        byte[] v = dc.GotBytes();
                        StaticContent sta = new StaticContent() { Buffer = v };
                        wc.Respond(200, sta, true, 60000);
                    }
                    else
                    {
                        wc.StatusCode = 404;
                    }
                }
                else // POST
                {
                    ArraySegment<byte>? bytes = wc.BytesSeg;
                    if (bytes == null)
                    {
                        wc.StatusCode = 301;
                    }
                    else if (dc.Execute("UPDATE fames SET icon = @1 WHERE id = @2", p => p.Put(bytes.Value).Put(id)) > 0)
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

        ///
        /// <code>
        /// GET /fame/_id_/img?idx=_num_
        /// </code>
        ///
        /// <code>
        /// POST /fame/_id_/img?idx=_num_
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
                            byte[] v = dc.GotBytes();
                            StaticContent sta = new StaticContent() { Buffer = v };
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
                            wc.StatusCode = 301; ;
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

    }
}