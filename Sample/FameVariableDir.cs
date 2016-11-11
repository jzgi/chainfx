using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    public class FameVariableDir : WebDir, IVariable
    {
        public FameVariableDir(WebDirContext ctx) : base(ctx)
        {
        }

        ///
        /// Get the record.
        ///
        /// <code>
        /// GET /fame/_id_/
        /// </code>
        ///
        public void @default(WebContext wc)
        {
            string id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                if (wc.IsGetMethod)
                {
                    const byte z = 0xff ^ BIN;
                    DbSql sql = new DbSql("SELECT ").columnlst(Fame.Empty, z)._("FROM fames WHERE id = @1");
                    if (dc.QueryA(sql.ToString(), p => p.Put(id)))
                    {
                        Fame obj = dc.ToData<Fame>(z);
                        wc.SendJson(200, obj, z);
                    }
                    else
                        wc.StatusCode = 404;
                }
            }
        }


        ///
        /// Update the record.
        ///
        /// <code>
        /// POST /fame/_id_/upd
        /// {
        ///   "name" : "michael",
        ///   "quote" : ""    
        /// }
        /// </code>
        public void upd(WebContext wc)
        {
            string uid = wc.Principal.Key;
            Fame fame = wc.ReadData<Fame>();
            fame.id = wc.Var;

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

        ///
        /// Get the icon.
        ///
        /// <code>
        /// GET /fame/_id_/icon
        /// </code>
        ///
        public void icon(WebContext wc)
        {
            string id = wc.Var;
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
        /// Update the icon.
        ///
        /// <code>
        /// POST /fame/_id_/updicon
        /// .....
        /// </code>
        ///
        public void updicon(WebContext wc)
        {
            string id = wc.Var;
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

        ///
        /// Get the nth image.
        ///
        /// <code>
        /// GET /fame/_id_/img[-_idx_]
        /// </code>
        ///
        public void img(WebContext wc, int idx)
        {
            string id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT m" + idx + " FROM fames WHERE id = @1", p => p.Put(id)))
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

        ///
        /// Update the nth image.
        ///
        /// <code>
        /// POST /fame/_id_/updimg[-_idx_]
        /// [img_bytes]
        /// </code>
        ///
        public void updimg(WebContext wc, int idx)
        {
            string id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                ArraySegment<byte>? bytes = wc.ReadByteA();
                if (bytes == null)
                {
                    wc.StatusCode = 301;
                }
                else if (dc.Execute("UPDATE posts SET m" + idx + " = @1 WHERE id = @2", p => p.Put(bytes.Value).Put(id)) > 0)
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