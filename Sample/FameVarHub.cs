using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public class FameVarHub : WebVarHub
    {
        public FameVarHub(WebArg arg) : base(arg)
        {
        }

        /// <summary>
        /// Get the record.
        /// </summary>
        /// <code>
        /// GET /fame/_id_/
        /// </code>
        ///
        public override void @default(WebContext wc, string var)
        {
            using (var dc = Service.NewDbContext())
            {
                if (wc.IsGet)
                {
                    if (dc.QueryA("SELECT * FROM fames WHERE id = @1", p => p.Put(var)))
                    {
                        Fame obj = dc.ToObj<Fame>();
                        wc.Out(200, obj);
                    }
                    else
                    {
                        wc.StatusCode = 404;
                    }
                }
            }
        }

        /// <summary>
        /// Update the record.
        /// </summary>
        /// <code>
        /// POST /fame/_id_/upd
        /// {
        /// }
        /// </code>
        public void upd(WebContext wc, string var)
        {
            Fame obj = wc.JObj.ToObj<Fame>();

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("INSERT INTO fames")._(obj)._VALUES_(obj)._("ON CONFLICT (id) DO UPDATE")._SET_(obj);
                if (dc.Execute(sql.ToString(), p => obj.Save(p)) > 0)
                {
                    wc.StatusCode = 200;
                }
                else
                {
                    wc.StatusCode = 400; // bad request
                }
            }
        }

        /// <summary>
        /// Get the icon.
        /// </summary>
        /// <code>
        /// GET /fame/_id_/icon
        /// </code>
        ///
        public void icon(WebContext wc, string var)
        {
            string id = var;
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT icon FROM fames WHERE id = @1", p => p.Put(id)))
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

        ///
        /// <summary>
        /// Update the icon.
        /// </summary>
        /// <code>
        /// POST /fame/_id_/updicon
        /// .....
        /// </code>
        ///
        public void updicon(WebContext wc, string var)
        {
            ArraySegment<byte>? bytes = wc.BytesSeg;
            string id = var;
            using (var dc = Service.NewDbContext())
            {
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

        /// <summary>
        /// Get the nth image.
        /// </summary>
        /// <code>
        /// GET /fame/_id_/img?idx=_n_
        /// </code>
        ///
        public void img(WebContext wc, string var)
        {
            int idx = 0;
            if (wc.Got(nameof(idx), ref idx))
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.QueryA("SELECT m" + idx + " FROM fames WHERE id = @1", p => p.Put(var)))
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
            else
            {
                wc.StatusCode = 304;
            }
        }

        /// <summary>
        /// Update the nth image.
        /// </summary>
        /// <code>
        /// POST /fame/_id_/updimg?idx=_n_
        /// [img_bytes]
        /// </code>
        ///
        public void updimg(WebContext wc, string var)
        {
            int idx = 0;
            if (wc.Got(nameof(idx), ref idx))
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
            else
            {
                wc.StatusCode = 304;
            }
        }
    }
}