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
                        var tel = dc.GetString();
                        var city = dc.GetString();
                        var distr = dc.GetString();
                        var addr = dc.GetString();
                        ac.GiveFormPane(200, m =>
                        {
                            m.TEXT(nameof(tel), tel, label: "电话");
                            m.SELECT(nameof(city), city, ((ShopService) Service).CityOpt, label: "城市");
                            //                            m.SELECT(nameof(distr), distr);
                            m.TEXT(nameof(addr), addr, label: "地址");
                        });
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                string shopid = ac[0];
                Form frm = await ac.ReadAsync<Form>();
                int[] pk = frm[nameof(pk)];

                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        var order = dc.ToArray<Order>();
                    }
                    else
                    {
                    }
                }
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
                if (dc.Query1("SELECT prepay_id, total FROM orders WHERE id = @1 AND custwx = @2", p => p.Set(id).Set(wx)))
                {
                    var prepay_id = dc.GetString();
                    var total = dc.GetDecimal();
                    if (prepay_id == null) // if not yet, call unifiedorder remotely
                    {
                        prepay_id = await WeiXinUtility.PostUnifiedOrderAsync(id, total, wx, ac.RemoteIpAddress.ToString(), "http://shop.144000.tv/notify");
                        dc.Execute("UPDATE orders SET prepay_id = @1 WHERE id = @2", p => p.Set(prepay_id).Set(id));
                    }

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

    public class OprPackedOrderVarWork : OprOrderVarWork
    {
        public OprPackedOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprSentOrderVarWork : OprOrderVarWork
    {
        public OprSentOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprDoneOrderVarWork : OprOrderVarWork
    {
        public OprDoneOrderVarWork(WorkContext wc) : base(wc)
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