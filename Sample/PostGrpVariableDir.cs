using System;
using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Greatbone.Sample
{
    public class PostGrpVariableDir : WebDir, IVariable
    {
        public PostGrpVariableDir(WebDirContext ctx) : base(ctx)
        {
        }

        ///
        /// Get the record.
        ///
        /// <code>
        /// GET /postgrp/_id_/
        /// </code>
        ///
        public void @default(WebContext wc)
        {
            string id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                const byte z = 0 ^ BIN;
                DbSql sql = new DbSql("SELECT ").columnlst(PostGrp.Empty, z)._("FROM postgrps WHERE id = @1");
                if (dc.QueryA(sql.ToString(), p => p.Put(id)))
                {
                    var postgrp = dc.ToData<PostGrp>(z);
                    wc.SendJson(200, postgrp, z);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
        }

        ///
        /// Get the icon image.
        ///
        /// <code>
        /// GET /post/_id_/icon
        /// </code>
        ///
        public void icon(WebContext wc)
        {
            string id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                if (dc.QueryA("SELECT icon FROM postgrps WHERE id = @1", p => p.Put(id)))
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
        /// Update the icon image.
        ///
        /// <code>
        /// POST /post/_id_/updicon
        /// [img_bytes]
        /// </code>
        ///
        [Check]
        public void updicon(WebContext wc)
        {
            string id = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                ArraySegment<byte>? bytes = wc.ReadByteA();
                if (bytes == null)
                {
                    wc.StatusCode = 301;
                }
                else if (dc.Execute("UPDATE posts SET icon= @1 WHERE id = @2", p => p.Put(bytes.Value).Put(id)) > 0)
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