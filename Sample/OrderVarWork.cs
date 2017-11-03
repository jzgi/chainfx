using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiMode;

namespace Greatbone.Sample
{
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

        [Ui("删除", Mode = ButtonConfirm)]
        public async Task rm(ActionContext ac)
        {
            string wx = ac[-2];
            var f = await ac.ReadAsync<Form>();
            long[] key = f[nameof(key)];
            using (var dc = ac.NewDbContext())
            {
                if (key != null)
                {
                    dc.Sql("DELETE FROM orders WHERE wx = @1 AND status = @2 AND id")._IN_(key);
                    dc.Execute(p => p.Set(wx).Set(Order.CREATED));
                }
                else
                {
                    dc.Execute("DELETE FROM orders WHERE wx = @1 AND status = @2", p => p.Set(wx).Set(Order.CREATED));
                }
                ac.GiveRedirect();
            }
        }

        [Ui("收货地址", Mode = ButtonShow, State = -3)]
        public async Task addr(ActionContext ac)
        {
            string wx = ac[-2];
            int id = ac[this];
            var f = await ac.ReadAsync<Form>();
            f.Let(out string area).Let(out string addr).Let(out string tel);
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE orders SET area = @1, addr = @2, tel = @3 WHERE id = @4", p => p.Set(area).Set(addr).Set(tel).Set(id));

                User prin = (User) ac.Principal;
                if (prin.city == null)
                {
                    dc.Execute("INSERT INTO users (wx, name, city, area, addr, tel, created) VALUES (@1, @2, @3, @4, @5, @6, @7) ON CONFLICT (wx) DO UPDATE SET name = @2, city = @3, distr = @4, addr = @5, tel = @6, created = @7", p => p.Set(wx).Set(area).Set(addr).Set(tel).Set(DateTime.Now));
                    prin.city = area;
                    prin.addr = addr;
                    prin.tel = tel;
                    // refresh the client cookie
                    ac.SetTokenCookie(prin, -1 ^ User.CREDENTIAL);
                }
            }
            ac.GivePane(200);
        }

        [Ui("付款", Mode = AScript, Em = true)]
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            int id = ac[this];

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

        [Ui("投诉", "向平台投诉该作坊的产品质量问题", Mode = AShow)]
        public async Task kick(ActionContext ac)
        {
            int id = ac[this];

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

        [Ui("确认收货", "对商品满意并确认收货", Mode = ButtonConfirm)]
        public async Task got(ActionContext ac)
        {
            int orderid = ac[this];
            string mgrwx = null;
            using (var dc = ac.NewDbContext())
            {
                var shopid = (string) dc.Scalar("UPDATE orders SET shipped = localtimestamp, status = @1 WHERE id = @2  RETURNING shopid", p => p.Set(Order.RECEIVED).Set(orderid));
                if (shopid != null)
                {
                    mgrwx = (string) dc.Scalar("SELECT mgrwx FROM shops WHERE id = @1", p => p.Set(shopid));
                }
            }
            if (mgrwx != null)
            {
                await WeiXinUtility.PostSendAsync(mgrwx, "【买家确收】订单编号：" + orderid);
            }

            ac.GiveRedirect("../");
        }
    }


    public class OprNewVarWork : OrderVarWork
    {
        public OprNewVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("撤单", "撤销此单，实收金额退回给买家", Mode = ButtonShow)]
        public async Task abort(ActionContext ac)
        {
            int orderid = ac[this];

            if (ac.GET)
            {
                ac.GivePane(200, m => { m.FORM_().CALLOUT("确定要撤销此单，实收金额退回给买家?")._FORM(); });
            }
            else
            {
                decimal total = 0, cash = 0;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT total, cash FROM orders WHERE id = @1", p => p.Set(orderid)))
                    {
                        dc.Let(out total).Let(out cash);
                    }
                }
                string err = await WeiXinUtility.PostRefundAsync(orderid, total, cash);
                if (err == null) // success
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(Order.ABORTED).Set(orderid));
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

    public class OprGoVarWork : OrderVarWork
    {
        public OprGoVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class OprPastVarWork : OrderVarWork
    {
        public OprPastVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("退款核查", "实时核查退款到账情况", Mode = AOpen)]
        public async Task refundq(ActionContext ac)
        {
            int orderid = ac[this];

            string err = await WeiXinUtility.PostRefundQueryAsync(orderid);
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

    public class AdmKickVarWork : OrderVarWork
    {
        public AdmKickVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}