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
        /// GET /fame/_var_/img?idx=_pic_idx_
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
        /// POST /fame/_var_/updimg?idx=_pic_idx_
        public void updimg(WebContext wc, string var)
        {
            int idx = 0;
            if (wc.Got(nameof(idx), ref idx))
            {
                ArraySegment<byte> bytes = wc.Bytes;
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Execute("UPDATE fames SET m" + idx + " = @1 WHERE id = @2", p => p.Put(var).Put(bytes.Array)) > 0)
                    {
                        wc.StatusCode = 200;
                    }
                    else
                    {
                        wc.StatusCode = 500;
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