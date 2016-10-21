using System;
using Greatbone.Core;

namespace Greatbone.Sample
{

    public class FameMultiple : WebMultiple
    {
        public FameMultiple(WebArg arg) : base(arg)
        {
        }

        /// <summary>
        /// Get the record.
        /// </summary>
        /// <code>
        /// GET /fame/_id_/
        /// </code>
        ///
        public override void @default(WebContext wc, string subscpt)
        {
            using (var dc = Service.NewDbContext())
            {
                if (wc.IsGet)
                {
                    if (dc.QueryA("SELECT * FROM fames WHERE id = @1", p => p.Put(subscpt)))
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
        public void upd(WebContext wc, string subscpt)
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
        public void icon(WebContext wc, string subscpt)
        {
            string id = wc.Super;
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
        public void updicon(WebContext wc, string subscpt)
        {
            string id = wc.Super;
            ArraySegment<byte>? bytes = wc.BytesSeg;
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
        public void img(WebContext wc, string subscpt)
        {
            string id = wc.Super;
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT m" + n + " FROM fames WHERE id = @1", p => p.Put(id)))
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
        /// POST /fame/_id_/updimg[-_n_]
        /// [img_bytes]
        /// </code>
        ///
        public void updimg(WebContext wc, string subscpt)
        {
            string id = wc.Super;
            int n = subscpt.ToInt();
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
    }
}