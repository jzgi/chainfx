using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public abstract class OrderVarWork : Work
    {
        protected OrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public abstract class MyOrderVarWork : OrderVarWork
    {
        protected MyOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }


    public class MyCartOrderVarWork : MyOrderVarWork
    {
        public MyCartOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("收货地址", UiMode.ButtonDialog)]
        public async Task addr(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT custtel, custcity, custdistr, custaddr FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        var custtel = dc.GetString();
                        var custcity = dc.GetString();
                        var custdistr = dc.GetString();
                        var custaddr = dc.GetString();
                        ac.GiveFormPane(200, m =>
                        {
                            m.TEXT(nameof(custtel), custtel, label: "电话");
                            m.SELECT(nameof(custcity), custcity, ((ShopService) Service).CityOpt, label: "城市");
                            //                            m.SELECT(nameof(distr), distr);
                            m.TEXT(nameof(custaddr), custaddr, label: "地址");
                        });
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                var frm = await ac.ReadAsync<Form>();
                string custtel = frm[nameof(custtel)];
                string custcity = frm[nameof(custcity)];
                string custdistr = frm[nameof(custdistr)];
                string custaddr = frm[nameof(custaddr)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET custtel = @1, custcity = @2, custdistr = @3, custaddr = @4 WHERE id = @5", p => p.Set(custtel).Set(custcity).Set(custdistr).Set(custaddr).Set(id));
                }
                ac.GiveRedirect("../");
            }
        }

        [Ui("附注", UiMode.ButtonDialog)]
        public async Task note(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT note FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        var note = dc.GetString();
                        ac.GiveFormPane(200, m => { m.TEXTAREA(nameof(note), note, label: "附加说明", max: 20, required: true); });
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                Form frm = await ac.ReadAsync<Form>();
                string note = frm[nameof(note)];

                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET note = @1 WHERE id = @2", p => p.Set(note).Set(id));
                }
                ac.GiveRedirect("../");
            }
        }

        static readonly Func<IData, bool> PREPAY = obj => ((Order) obj).custaddr != null;

        [Ui("付款", UiMode.AnchorScript, Alert = true)]
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT total FROM orders WHERE id = @1 AND custwx = @2", p => p.Set(id).Set(wx)))
                {
                    var total = dc.GetDecimal();
                    var prepay_id = await WeiXinUtility.PostUnifiedOrderAsync(id, total, wx, ac.RemoteAddr, "http://shop.144000.tv/notify");
                    ac.Give(200, WeiXinUtility.BuildPrepayContent(prepay_id));
                }
                else
                {
                    ac.Give(404, "order not found");
                }
            }
        }
    }

    public class MyCurrentOrderVarWork : MyOrderVarWork
    {
        public MyCurrentOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("撤销")]
        public async Task abort(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        var order = dc.ToArray<Order>();
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id IN () AND shopid = @1 AND ").statecond();
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        ac.Give(303); // see other
                    }
                    else
                    {
                        ac.Give(303); // see other
                    }
                }
            }
        }
    }

    public class MyHistoryOrderVarWork : MyOrderVarWork
    {
        public MyHistoryOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public abstract class OprOrderVarWork : OrderVarWork
    {
        protected OprOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprCreatedOrderVarWork : OprOrderVarWork
    {
        public OprCreatedOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprPaidOrderVarWork : OprOrderVarWork
    {
        public OprPaidOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }
    }

    public class OprCartOrderVarWork : OprOrderVarWork
    {
        public OprCartOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprSentOrderVarWork : OprOrderVarWork
    {
        public OprSentOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprHistoryOrderVarWork : OprOrderVarWork
    {
        public OprHistoryOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprAbortedOrderVarWork : OprOrderVarWork
    {
        public OprAbortedOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public abstract class DvrOrderVarWork : OrderVarWork
    {
        protected DvrOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class DvrSentOrderVarWork : DvrOrderVarWork
    {
        public DvrSentOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class DvrDoneOrderVarWork : DvrOrderVarWork
    {
        public DvrDoneOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}