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


    public class MyPreVarWork : OrderVarWork
    {
        public MyPreVarWork(WorkContext wc) : base(wc)
        {
        }

        public bool NoAddr(object obj) => string.IsNullOrEmpty(((Order) obj).addr);

        [Ui("填写收货地址", Mode = UiMode.ButtonShow)]
        public async Task addr(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];
            string buyer;
            string city;
            string addr;
            string tel;
            bool save;
            if (ac.GET)
            {
                var f = ac.Query;
                f.Let(out buyer).Let(out city).Let(out addr).Let(out tel).Let(out save);

                if (city == null)
                {
                    using (var dc = ac.NewDbContext())
                    {
                        if (dc.Query1("SELECT buyer, city, distr, addr, tel FROM orders WHERE id = @1", p => p.Set(id)))
                        {
                            dc.Let(out buyer).Let(out city).Let(out addr).Let(out tel);
                        }
                    }
                }
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(buyer), buyer, label: "买家名称", max: 10, required: true);
                    m.SELECT(nameof(city), city, ((SampleService) Service).Cities, label: "城市", refresh: true);
                    m.TEXT(nameof(addr), addr, label: "地址", pattern: "[\\S]*", max: 20, required: true);
                    m.TEXT(nameof(tel), tel, label: "电话", max: 11, required: true);
                    m.CHECKBOX(nameof(save), save, label: "存为默认的收货地址");
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                f.Let(out buyer).Let(out city).Let(out addr).Let(out tel).Let(out save);
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET buyer = @1, city = @2, distr = @3, addr = @4, tel = @5 WHERE id = @6", p => p.Set(buyer).Set(city).Set(addr).Set(tel).Set(id));
                    if (save)
                    {
                        User prin = (User) ac.Principal;
                        dc.Execute("INSERT INTO users (wx, name, city, distr, addr, tel, created) VALUES (@1, @2, @3, @4, @5, @6, @7) ON CONFLICT (wx) DO UPDATE SET name = @2, city = @3, distr = @4, addr = @5, tel = @6, created = @7", p => p.Set(wx).Set(buyer).Set(city).Set(addr).Set(tel).Set(DateTime.Now));
                        prin.name = buyer;
                        prin.city = city;
                        prin.addr = addr;
                        prin.tel = tel;
                        ac.SetTokenCookie(prin, -1 ^ User.CREDENTIAL);
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

        [Ui("付款", "确定要通过微信付款吗?一经确认，该单金额将不能作更改。", Mode = UiMode.AnchorScript, Bold = true)]
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            long id = ac[this];

            decimal total = 0;
            using (var dc = ac.NewDbContext())
            {
                total = (decimal) dc.Scalar("SELECT total FROM orders WHERE id = @1 AND wx = @2", p => p.Set(id).Set(wx));
            }
            string prepay_id = await WeiXinUtility.PostUnifiedOrderAsync(id, total, wx, ac.RemoteAddr, "http://shop.144000.tv/notify");
            ac.Give(200, WeiXinUtility.BuildPrepayContent(prepay_id));
        }
    }

    public class MyOrderVarWork : OrderVarWork
    {
        public MyOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("申请撤销", "向商家申请撤销此单", Mode = UiMode.ButtonPrompt)]
        public async Task cancel(ActionContext ac)
        {
            long id = ac[this];
            string abortion;
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
                await WeiXinUtility.PostSendAsync(mgrwx, "【买家确收】订单编号：" + id);
            }

            ac.GiveRedirect("../");
        }
    }


    public class OprNewOrderVarWork : OrderVarWork
    {
        public OprNewOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        public bool NoAbortion(object obj) => string.IsNullOrEmpty(((Order) obj).abortly);

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

    public class OprOnOrderVarWork : OrderVarWork
    {
        public OprOnOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        public bool NotAborted(object obj) => ((Order) obj).status != Order.ABORTED;

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
                    m.CALLOUT(err);
                    m.CHECKBOX("ok", false, "重新提交退款请求", true);
                    m.BUTTON("", 1, "确认");
                    m._FORM();
                });
            }
        }
    }

    public class OprPastOrderVarWork : OrderVarWork
    {
        public OprPastOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        public bool NotAborted(object obj) => ((Order) obj).status != Order.ABORTED;

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
                    m.CALLOUT(err);
                    m.CHECKBOX("ok", false, "重新提交退款请求", true);
                    m.BUTTON("", 1, "确认");
                    m._FORM();
                });
            }
        }
    }
}