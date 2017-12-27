using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;

namespace Greatbone.Samp
{
    public abstract class ShopWork<V> : Work where V : ShopVarWork
    {
        protected ShopWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Shop) obj).id);
        }
    }

    [Ui("网点")]
    public class AdmShopWork : ShopWork<AdmShopVarWork>
    {
        public AdmShopWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Query(dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops ORDER BY id"));
                ac.GiveBoardPage(200, dc.ToArray<Shop>(), (h, o) =>
                {
                    h.CAPTION_().T(o.name).T(" / ").T(o.id)._CAPTION();
                    h.FIELD_("地址").T(o.city)._T(o.addr)._FIELD();
                    h.FIELD_("坐标").T(o.x)._T(o.y)._FIELD();
                    h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                    h.TAIL();
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(ActionContext ac)
        {
            const short proj = Shop.ADM;
            if (ac.GET)
            {
                var o = new Shop {city = City.All[0].name};
                o.Read(ac.Query, proj);
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.id), o.id, "编号", max: 4, min: 4, required: true);
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.SELECT(nameof(o.city), o.city, City.All, "城市", refresh: true);
                    m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度", max: 20, box: 6).NUMBER(nameof(o.x), o.x, "纬度", max: 20, box: 6);
                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Shop>(proj);
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(dc.Sql("INSERT INTO shops")._(Shop.Empty, proj)._VALUES_(Shop.Empty, proj), p => o.Write(p, proj));
                }
                ac.GivePane(200); // created
            }
        }
    }
}