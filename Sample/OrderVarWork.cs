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
            CreateVar<MyCartOrderVarVarWork, string>(obj => ((OrderLine) obj).item);
        }

        [Ui("收货地址", Mode = UiMode.ButtonDialog)]
        public async Task addr(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            string tel = null;
            string city = null;
            string distr = null;
            string addr = null;

            if (ac.GET)
            {
                tel = ac.Query[nameof(tel)];
                city = ac.Query[nameof(city)];
                distr = ac.Query[nameof(distr)];
                addr = ac.Query[nameof(addr)];

                if (city == null)
                {
                    using (var dc = ac.NewDbContext())
                    {
                        if (dc.Query1("SELECT custtel, custcity, custdistr, custaddr FROM orders WHERE id = @1", p => p.Set(id)))
                        {
                            tel = dc.GetString();
                            city = dc.GetString();
                            distr = dc.GetString();
                            addr = dc.GetString();
                        }
                    }
                }
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(tel), tel, label: "电话");
                    m.SELECT(nameof(city), city, ((ShopService) Service).CityOpt, label: "城市", refresh: true);
                    m.SELECT(nameof(distr), distr, ((ShopService) Service).GetDistrs(city), label: "区域");
                    m.TEXT(nameof(addr), addr, label: "地址");
                    m._FORM();
                });
            }
            else
            {
                var frm = await ac.ReadAsync<Form>();
                tel = frm[nameof(tel)];
                city = frm[nameof(city)];
                distr = frm[nameof(distr)];
                addr = frm[nameof(addr)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET custtel = @1, custcity = @2, custdistr = @3, custaddr = @4 WHERE id = @5", p => p.Set(tel).Set(city).Set(distr).Set(addr).Set(id));
                }
                ac.GiveRedirect("../");
            }
        }

        [Ui("附注", Mode = UiMode.ButtonDialog)]
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
                        ac.GivePane(200, m => { m.TEXTAREA(nameof(note), note, label: "附加说明", max: 20, required: true); });
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

        [Ui("付款", Mode = UiMode.AnchorScript, Alert = true)]
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

    public class MyPresentOrderVarWork : MyOrderVarWork
    {
        public MyPresentOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("请求撤销")]
        public void abort(ActionContext ac)
        {
            long id = ac[this];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(0).Set(id));
            }
        }
    }

    public class MyPastOrderVarWork : MyOrderVarWork
    {
        public MyPastOrderVarWork(WorkContext wc) : base(wc)
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

    public class OprAcceptedOrderVarWork : OprOrderVarWork
    {
        public OprAcceptedOrderVarWork(WorkContext wc) : base(wc)
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

        [Ui("置己收", Mode = UiMode.ButtonConfirm)]
        public async Task rcved(ActionContext ac)
        {
            long id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(Order.ACCEPTED).Set(id));
                ac.GiveRedirect("../");
            }
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

    public class OprAlienOrderVarWork : OprOrderVarWork
    {
        public OprAlienOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}