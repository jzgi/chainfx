using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
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
            CreateVar<MyCartOrderVarVarWork, string>(obj => ((OrderLine) obj).name);
        }

        [Ui("收货地址", Mode = UiMode.ButtonPrompt)]
        public async Task addr(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            string city;
            string distr;
            string addr;
            string tel;

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
                        if (dc.Query1("SELECT city, distr, addr, tel FROM orders WHERE id = @1", p => p.Set(id)))
                        {
                            dc.GetOn(ref city, ref distr, ref addr, ref tel);
                        }
                    }
                }
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.SELECT(nameof(city), city, ((ShopService) Service).CityOpt, label: "城市", refresh: true);
                    m.SELECT(nameof(distr), distr, ((ShopService) Service).GetDistrs(city), label: "区域");
                    m.TEXT(nameof(addr), addr, label: "地址");
                    m.TEXT(nameof(tel), tel, label: "电话");
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
                    dc.Execute("UPDATE orders SET tel = @1, city = @2, distr = @3, addr = @4 WHERE id = @5", p => p.Set(tel).Set(city).Set(distr).Set(addr).Set(id));
                }
                ac.GiveRedirect("../");
            }
        }

        [Ui("附加说明", Mode = UiMode.ButtonPrompt)]
        public async Task note(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT comment FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        var comment = dc.GetString();
                        ac.GivePane(200, m => { m.TEXTAREA(nameof(comment), comment, label: "附加说明", max: 20, required: true); });
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                Form f = await ac.ReadAsync<Form>();
                string comment = f[nameof(comment)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET comment = @1 WHERE id = @2", p => p.Set(comment).Set(id));
                }
                ac.GiveRedirect("../");
            }
        }

        static readonly Func<IData, bool> PREPAY = obj => ((Order) obj).addr != null;

        [Ui("付款", "确定此单要付款吗", Mode = UiMode.AnchorScript, Bold = true)]
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT total FROM orders WHERE id = @1 AND wx = @2", p => p.Set(id).Set(wx)))
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

    public class MyActiveOrderVarWork : MyOrderVarWork
    {
        public MyActiveOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("请求撤销", Mode = UiMode.ButtonPrompt)]
        public async Task cancel(ActionContext ac)
        {
            long id = ac[this];
            string abortion = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXTAREA(nameof(abortion), abortion, "请填写撤销的原因", max: 20, required: true);
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                abortion = f[nameof(abortion)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET abortion = @1 WHERE id = @2", p => p.Set(abortion).Set(id));
                }
                ac.GiveRedirect("../");
            }
        }

        [Ui("确认收货", "对商品满意并确认收货", Mode = UiMode.ButtonConfirm)]
        public void got(ActionContext ac)
        {
            long id = ac[this];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE orders SET shipped = localtimestamp, status = @1 WHERE id = @2", p => p.Set(Order.SHIPPED).Set(id));
            }
            ac.GiveRedirect("../");
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

    public class OprCartOrderVarWork : OprOrderVarWork
    {
        public OprCartOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("强行接受", "此单可能尚未付款，确定接受吗", Mode = UiMode.ButtonConfirm)]
        public void accept(ActionContext ac)
        {
            long id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE orders SET accepted = localtimestamp, status = @1 WHERE id = @2", p => p.Set(Order.ACCEPTED).Set(id));
                ac.GiveRedirect("../");
            }
        }

        [Ui("付款核查", Mode = UiMode.ButtonConfirm)]
        public async Task check(ActionContext ac)
        {
            long id = ac[this];

            decimal cash = await WeiXinUtility.PostOrderQueryAsync(id);
            if (cash > 0)
            {
                using (var dc = Service.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET cash = @1 WHERE id = @2", p => p.Set(cash).Set(id));
                }
            }
            ac.GiveRedirect("../");
        }
    }

    public class OprActiveOrderVarWork : OprOrderVarWork
    {
        public OprActiveOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        static readonly Func<IData, bool> ABORT = obj => ((Order) obj).abortion != null;

        [Ui("撤销", "撤销此单，实收金额将会退回给买家", Mode = UiMode.ButtonConfirm)]
        public async Task abort(ActionContext ac)
        {
            long id = ac[this];

            decimal total = 0, cash = 0;
            if (await WeiXinUtility.PostRefundAsync(id, total, cash))
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(Order.ABORTED).Set(id));
                }
            }
            ac.GiveRedirect("../");
        }
    }

    public class OprPastOrderVarWork : OprOrderVarWork
    {
        public OprPastOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        static readonly Func<IData, bool> ABORT = obj => ((Order) obj).abortion != null;

        [Ui("退款核实", "撤销此单，实收金额将会退回给买家", Mode = UiMode.AnchorShow)]
        public async Task abort(ActionContext ac)
        {
            long id = ac[this];

            decimal total, cash;
            if (await WeiXinUtility.PostRefundQueryAsync(id))
            {
            }
            ac.GiveRedirect("../");
        }
    }

    public class OprPartnerOrderVarWork : OprOrderVarWork
    {
        public OprPartnerOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}