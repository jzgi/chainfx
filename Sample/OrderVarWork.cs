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

        [Ui("收货地址", Mode = UiMode.ButtonShow)]
        public async Task addr(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];
            string city = null;
            string distr = null;
            string addr = null;
            string tel = null;
            bool save = true;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT city, distr, addr, tel FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        dc.Let(out city).Let(out distr).Let(out addr).Let(out tel);
                    }
                }
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.SELECT(nameof(city), city, ((ShopService) Service).CityOpt, label: "城市", refresh: true);
                    m.SELECT(nameof(distr), distr, ((ShopService) Service).GetDistrs(city), label: "区域");
                    m.TEXT(nameof(addr), addr, label: "地址");
                    m.TEXT(nameof(tel), tel, label: "电话");
                    m.CHECKBOX(nameof(save), save, label: "存为默认的收货地址");
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                f.Let(out city).Let(out distr).Let(out addr).Let(out tel).Let(out save);
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET city = @1, distr = @2, addr = @3, tel = @4 WHERE id = @5", p => p.Set(city).Set(distr).Set(addr).Set(tel).Set(id));
                    if (save)
                    {
                        User prin = (User) ac.Principal;
                        dc.Execute("INSERT INTO users (wx, nickname, city, distr, addr, tel, created) VALUES (@1, @2, @3, @4, @5, @6, @7) ON CONFLICT (wx) DO UPDATE SET nickname = @2, city = @3, distr = @4, addr = @5, tel = @6, created = @7", p => p.Set(wx).Set(prin.nickname).Set(city).Set(distr).Set(addr).Set(tel).Set(DateTime.Now));
                        prin.city = city;
                        prin.distr = distr;
                        prin.addr = addr;
                        prin.tel = tel;
                        ac.SetTokenCookie(prin, 0xffff ^ User.CREDENTIAL);
                    }
                }
                ac.GivePane(200);
            }
        }

        [Ui("附加说明", Mode = UiMode.ButtonShow)]
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
                        string note;
                        dc.Let(out note);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.TEXTAREA(nameof(note), note, label: "附加说明", max: 20, required: true);
                            m._FORM();
                        });
                    }
                    else
                    {
                        ac.Give(404);
                    }
                }
            }
            else
            {
                Form f = await ac.ReadAsync<Form>();
                string note = f[nameof(note)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET note = @1 WHERE id = @2", p => p.Set(note).Set(id));
                }
                ac.GivePane(200);
            }
        }

        static readonly Func<IData, bool> PREPAY = obj => !string.IsNullOrEmpty(((Order) obj).addr);

        [Ui("微信付款", "确定要通过微信付款吗", Mode = UiMode.AnchorScript, Bold = true)]
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT total FROM orders WHERE id = @1 AND wx = @2", p => p.Set(id).Set(wx)))
                {
                    decimal total;
                    dc.Let(out total);
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

        [Ui("申请撤销", "向商家申请撤销此单", Mode = UiMode.ButtonPrompt)]
        public async Task cancel(ActionContext ac)
        {
            long id = ac[this];
            string abortion = null;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    abortion = (string) dc.Scalar("SELECT abortion FROM orders WHERE id = @1", p => p.Set(id));
                }
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXTAREA(nameof(abortion), abortion, "撤销此单的理由", max: 20, required: true);
                    m.CALLOUT("一经商家同意，您通过平台支付的金额将在20分钟之内退回您的钱包。如果是从银行卡支付的，则可能需要更长时间。如经同意后两天内没有收到退款，请与商家联系。");
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
        public async Task got(ActionContext ac)
        {
            long id = ac[this];
            string mgrwx = null;
            using (var dc = ac.NewDbContext())
            {
                var shopid = (string) dc.Scalar("UPDATE orders SET shipped = localtimestamp, status = @1 WHERE id = @2  RETURNING shopid", p => p.Set(Order.SHIPPED).Set(id));
                if (shopid != null)
                {
                    mgrwx = (string) dc.Scalar("SELECT mgrwx FROM shops WHERE id = @1", p => p.Set(shopid));
                }
            }
            if (mgrwx != null)
            {
                await WeiXinUtility.PostSendAsync(mgrwx, "[买家确收]订单编号：" + id);
            }

            ac.GiveRedirect("../");
        }
    }

    public class MyPastOrderVarWork : MyOrderVarWork
    {
        public MyPastOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("举报商家", "向平台举报商家的产品质量问题", Mode = UiMode.AnchorShow)]
        public async Task tipoff(ActionContext ac)
        {
            long id = ac[this];

            bool yes = false;
            string report = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();

                    m.CALLOUT("让我们共同来维护人类健康福祉。在监管人员受理您的举报后，可能需要在您的协助下进行调查。", false);
                    m.CHECKBOX(nameof(yes), yes, "我同意协助监管人员进行调查", required: true);
                    m.TEXTAREA(nameof(report), report, "举报内容", max: 40, required: true);

                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                report = f[nameof(report)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("INSERT INTO charges () VALUES () ON CONFLICT DO NOTHING");
                }
                ac.GivePane(200);
            }
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

        [Ui("强行接受", "此单可能尚未通过平台付款，确定接受吗", Mode = UiMode.ButtonConfirm)]
        public void accept(ActionContext ac)
        {
            long id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE orders SET accepted = localtimestamp, status = @1 WHERE id = @2", p => p.Set(Order.ACCEPTED).Set(id));
                ac.GiveRedirect("../");
            }
        }

        [Ui("付款情况核查", "实时核查该单的付款情况", Mode = UiMode.ButtonConfirm)]
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

        [Ui("同意撤销/退款", "同意撤销此单，实收金额退回给买家", Mode = UiMode.ButtonShow)]
        public async Task abort(ActionContext ac)
        {
            long id = ac[this];

            if (ac.GET)
            {
                ac.GivePane(200, m => { m.FORM_().CALLOUT("确定要撤销此单，实收金额退回给买家?")._FORM(); });
            }
            else
            {
                decimal total = 0, cash = 0;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT total, cash FROM orders WHERE id = @1", p => p.Set(id)))
                    {
                        dc.Let(out total).Let(out cash);
                    }
                }
                string err = await WeiXinUtility.PostRefundAsync(id, total, cash);
                if (err == null) // success
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(Order.ABORTED).Set(id));
                    }
                    ac.GivePane(200);
                }
                else // err
                {
                    ac.GivePane(200, m => { m.FORM_().CALLOUT(err).CALLOUT("确定重复操作吗？")._FORM(); });
                }
            }
        }
    }

    public class OprPastOrderVarWork : OprOrderVarWork
    {
        public OprPastOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        static readonly Func<IData, bool> REFUNDQ = obj => ((Order) obj).status == Order.ABORTED;

        [Ui("退款情况核查", "实时核查退款到账情况", Mode = UiMode.AnchorOpen)]
        public async Task refundq(ActionContext ac)
        {
            long id = ac[this];

            string err = await WeiXinUtility.PostRefundQueryAsync(id);
            if (err == null) // success
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.CALLOUT("退款成功", false);
                    m._FORM();
                });
            }
            else
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.CALLOUT(err, false);
                    m.CHECKBOX("ok", false, "重新提交退款请求", true);
                    m.BUTTON("", 1, "确认");
                    m._FORM();
                });
            }
        }
    }

    public class OprCoOrderVarWork : OprOrderVarWork
    {
        public OprCoOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}