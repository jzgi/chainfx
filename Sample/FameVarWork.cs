using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{

    public class FameMuxWork : WebWork
    {
        public FameMuxWork(WebWorkContext wwc) : base(wwc)
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
            string id = wc[0];
            using (var dc = Service.NewDbContext())
            {
                if (wc.IsGetMethod)
                {
                    const byte z = 0xff ^ BIN;
                    DbSql sql = new DbSql("SELECT ").columnlst(Fame.Empty, z)._("FROM fames WHERE id = @1");
                    if (dc.QueryA(sql.ToString(), p => p.Put(id)))
                    {
                        Fame obj = dc.ToBean<Fame>(z);
                        wc.SendJson(200, obj, z);
                    }
                    else
                        wc.StatusCode = 404;
                }
            }
        }



        /// <summary>
        /// Update the record.
        /// </summary>
        /// <code>
        /// POST /fame/_id_/upd
        /// {
        ///   "name" : "michael",
        ///   "quote" : ""    
        /// }
        /// </code>
        public void upd(WebContext wc, string subscpt)
        {
            string uid = wc.Principal.Key;
            Fame fame = wc.ReadBean<Fame>();
            fame.id = wc[0];

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("INSERT INTO fames")._(Fame.Empty)._VALUES_(Fame.Empty)._("ON CONFLICT (id) DO UPDATE")._SET_(Fame.Empty)._("WHERE fames.id = @1");
                if (dc.Execute(sql.ToString(), p => { fame.Dump(p); p.Put(uid); }) > 0)
                {
                    wc.StatusCode = 200; // ok
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
            string id = wc[0];
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT icon FROM fames WHERE id = @1", p => p.Put(id)))
                {
                    byte[] v = dc.GetBytes();
                    StaticContent sta = new StaticContent()
                    {
                        ByteBuffer = v
                    };
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
            string id = wc[0];
            ArraySegment<byte>? bytes = wc.ReadByteA();
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
            string id = wc[0];
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT m" + n + " FROM fames WHERE id = @1", p => p.Put(id)))
                {
                    byte[] v = dc.GetBytes();
                    StaticContent sta = new StaticContent() { ByteBuffer = v };
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
            string id = wc[0];
            int n = subscpt.ToInt();
            using (var dc = Service.NewDbContext())
            {
                ArraySegment<byte>? bytes = wc.ReadByteA();
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