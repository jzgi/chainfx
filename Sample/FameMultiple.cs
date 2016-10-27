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
            string id = wc.Super;
            using (var dc = Service.NewDbContext())
            {
                if (wc.IsGet)
                {
                    if (dc.QueryA("SELECT * FROM fames WHERE id = @1", p => p.Put(id)))
                    {
                        Fame obj = dc.ToObj<Fame>();
                        wc.SendJ(200, obj);
                    }
                    else
                    {
                        wc.StatusCode = 404;
                    }
                }
            }
        }



        static string UpdSql = new DbSql("INSERT INTO fames")._(new Fame())._VALUES_(new Fame())._("ON CONFLICT (id) DO UPDATE")._SET_(new Fame())._("WHERE authorid = @1").ToString();

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
            string uid = wc.Token.Key;
            Fame obj = wc.Obj<Fame>();
            obj.id = wc.Super;

            using (var dc = Service.NewDbContext())
            {
                if (dc.Execute(UpdSql, p => { obj.Save(p); p.Put(uid); }) > 0)
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
                    wc.Send(200, sta, true, 60000);
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
                    wc.Send(200, sta, true, 60000);
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