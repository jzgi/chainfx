using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/
    ///
    public class ShopFolder : WebFolder
    {
        public ShopFolder(WebFolderContext fc) : base(fc)
        {
            CreateVar<ShopVarFolder>();
        }

        // whether an aligned floating point
        bool IsAligned(string v)
        {
            int pt = v.IndexOf('.');
            return pt == v.Length - 2;
        }

        ///
        /// Get items grouped by shop
        ///
        /// <code>
        /// GET /items
        /// </code>
        ///
        public void lst(WebActionContext ac)
        {
            string x = ac.Query[nameof(x)];
            string y = ac.Query[nameof(y)];
            if (!IsAligned(x) || !IsAligned(y))
            {
                ac.Reply(400, "x or y not aligned");
                return;
            }

            double dx, dy;
            dx = double.Parse(x);
            dy = double.Parse(y);

            double x1 = dx - 0.1;
            double x2 = dx + 0.2;

            double y1 = dy - 0.1;
            double y2 = dy + 0.2;

            // get nearby shops
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT shops.*, items.* FROM shops INNER JOIN items ON shops.id = items.shopid WHERE x > @1 AND x < @2 AND y > @3 AND y < @4", p => p.Set(x1).Set(x2).Set(y1).Set(y2)))
                {
                    ac.Reply(200, dc.Dump<JsonContent>());
                }
                else
                {
                    ac.Reply(204); // no content
                }
            }
        }

        //
        // administrative actions
        //

        [Admin]
        [Ui]
        public void @default(WebActionContext ac)
        {

        }

        // [Admin]
        [Ui("新建")]
        public async Task @new(WebActionContext ac)
        {
            if (ac.GET)
            {
                Shop o = Shop.Empty;
                ac.ReplyDlg(200, a =>
                {
                    a.FIELDSET_("新建服务点");
                    a.TEXT(nameof(o.id), o.id, new Ui<sbyte> { });
                    a.TEXT(nameof(o.name), o.name);
                    a.PASSWORD("password", "");
                    a.TEL(nameof(o.tel), o.tel);
                    a.CHECKBOX("ok", true);
                    a._FIELDSET();
                });
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>();

                // validate


                using (var dc = Service.NewDbContext())
                {
                    shop.credential = TextUtility.MD5(shop.id + ':' + shop.credential);
                    dc.Sql("INSERT INTO shops")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(p => p.Set(shop)) > 0)
                    {
                        ac.Reply(201); // created
                    }
                    else
                    {
                        ac.Reply(500); // internal server error
                    }
                }
            }
        }

        ///
        /// Get buyer's personal order list
        ///
        public void all(WebActionContext ac)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void list(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        ///
        /// find in shop's order list
        ///
        public void clear(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

    }
}