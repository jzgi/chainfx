using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    public abstract class CityWork<V> : Work where V : CityVarWork
    {
        protected CityWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("申请", UiMode.AnchorDialog)]
        public async Task apply(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GiveFormPane(200, Shop.Empty, -1 ^ TRANSF);
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>();

                // validate

                using (var dc = Service.NewDbContext())
                {
                    shop.credential = StrUtility.MD5(shop.id + ':' + shop.credential);
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

        [Ui("新建", UiMode.AnchorDialog)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                ac.GiveFormPane(200, Shop.Empty, -1 ^ TRANSF);
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>();

                // validate

                using (var dc = Service.NewDbContext())
                {
                    shop.credential = StrUtility.MD5(shop.id + ':' + shop.credential);
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
    }

    /// <summary>
    ///
    /// </summary>
    public class MgrCityWork : CityWork<MgrCityVarWork>
    {
        public MgrCityWork(WorkContext wc) : base(wc)
        {
            CreateVar<MgrCityVarWork, string>((prin) => ((User) prin).mgrat);
        }
    }


    [Ui("城市")]
    public class AdmCityWork : CityWork<AdmCityVarWork>
    {
        public AdmCityWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query(p => p.Set(page)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Shop>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Shop>) null);
                }
            }
        }
    }
}