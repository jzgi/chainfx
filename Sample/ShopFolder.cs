using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    public class ShopFolder : Folder
    {
        public ShopFolder(FolderContext fc) : base(fc)
        {
            CreateVar<ShopVarFolder>((tok) => ((User)tok).shopid);
        }

        // [Shop]
        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ BIN ^ CODE ^ SECRET;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query(p => p.Set(page)))
                {
                    ac.GiveFolderPage(Parent, 200, dc.ToList<Shop>(proj), proj);
                }
                else
                {
                    ac.GiveFolderPage(Parent, 200, (List<Shop>)null);
                }
            }
        }

        // whether an aligned floating point
        bool IsAligned(string v)
        {
            if (v == null) { return false; }

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
        public void lst(ActionContext ac)
        {
            string x = ac.Query[nameof(x)];
            string y = ac.Query[nameof(y)];
            if (!IsAligned(x) || !IsAligned(y))
            {
                ac.Give(400, "x or y not aligned");
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
                if (dc.Query("SELECT * FROM shops WHERE x > @1 AND x < @2 AND y > @3 AND y < @4", p => p.Set(x1).Set(x2).Set(y1).Set(y2)))
                {
                    ac.Give(200, dc.Dump());
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }

        //
        // administrative actions
        //

        [Ui("申请", Dialog = 1)]
        public async Task apply(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GiveDialogForm(200, Shop.Empty, -1 ^ Projection.CODE);
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
                        ac.Give(201); // created
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }

        [User()]
        [Ui("新建", Dialog = 1)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GiveDialogForm(200, Shop.Empty, -1 ^ Projection.CODE);
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
                        ac.Give(201); // created
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }

        [Ui("停业/启用")]
        public void toggle(ActionContext ac)
        {

        }

        [Ui("分布报告")]
        public void rpt(ActionContext ac)
        {

        }

        public async Task @null(ActionContext ac)
        {
            if (ac.GET)
            {
                // return a form
                // ac.GivePage(200, )

            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                string shopid = f[nameof(shopid)];
                string password = f[nameof(password)];
                string orig = f[nameof(orig)];

                // data op
                User prin = (User)ac.Principal;
                if (prin.temp)
                {
                    prin.shopid = shopid;
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Sql("INSERT INTO users")._(User.Empty)._VALUES_(User.Empty)._("");
                        dc.Execute("INSERT INTO users");
                    }
                }
                else
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET shopid = @1 WHERE id = @2");
                    }
                }

                // return back
                ac.GiveRedirect(orig);
            }
        }
    }
}