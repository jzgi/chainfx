using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/
    ///
    [Authorize]
    public class ShopFolder : Folder
    {
        public ShopFolder(FolderContext fc) : base(fc)
        {
            CreateVar<ShopVarFolder>();
        }

        [User]
        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM shops"))
                {
                    ac.ReplyFolderPage(200, dc.ToList<Shop>()); // ok
                }
                else
                {
                    ac.Reply(204); // no content
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
                if (dc.Query("SELECT * FROM shops WHERE x > @1 AND x < @2 AND y > @3 AND y < @4", p => p.Set(x1).Set(x2).Set(y1).Set(y2)))
                {
                    ac.Reply(200, dc.Dump());
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

        // [Admin]
        [Ui("新建", 3)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                Shop o = Shop.Empty;
                ac.ReplyForm(200, o);
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

        [Ui("删除", 1)]
        public void del(ActionContext ac)
        {

        }

        [Ui("禁用", 2)]
        public void disable(ActionContext ac)
        {

        }

        [Ui("启用", 2)]
        public void enable(ActionContext ac)
        {

        }

        [Ui("分布报告", 2)]
        public void rpt(ActionContext ac)
        {

        }
    }
}