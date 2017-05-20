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

        [Ui("附注", Mode = UiMode.ButtonPrompt)]
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
                Form f = await ac.ReadAsync<Form>();
                string note = f[nameof(note)];

                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET note = @1 WHERE id = @2", p => p.Set(note).Set(id));
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
            string reason = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXTAREA(nameof(reason), reason, "请填写撤销的原因");
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                reason = f[nameof(reason)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET cancel = @1 WHERE id = @2", p => p.Set(reason).Set(id));
                }
            }
        }

        [Ui("确认收货", "确认收货并结束次单", Mode = UiMode.ButtonPrompt)]
        public async Task got(ActionContext ac)
        {
            long id = ac[this];
            string review = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXTAREA(nameof(review), review, "请给出您的宝贵意见");
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                review = f[nameof(review)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET review = @1 WHERE id = @2", p => p.Set(review).Set(id));
                }
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
                dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(Order.ACCEPTED).Set(id));
                ac.GiveRedirect("../");
            }
        }

        [Ui("付款核查", Mode = UiMode.AnchorShow)]
        public void check(ActionContext ac)
        {
            long id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(Order.ACCEPTED).Set(id));
                ac.GiveRedirect("../");
            }
        }
    }

    public class OprActiveOrderVarWork : OprOrderVarWork
    {
        public OprActiveOrderVarWork(WorkContext wc) : base(wc)
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
                    var order = dc.ToDatas<Order>();
                }
                else
                {
                }
            }
        }
    }

    public class OprPastOrderVarWork : OprOrderVarWork
    {
        public OprPastOrderVarWork(WorkContext wc) : base(wc)
        {
        }

        static readonly Func<IData, bool> UNCLOSE = obj => ((Order) obj).status < Order.CLOSED;

        [Ui("反关闭")]
        public void unclose(ActionContext ac)
        {
            long[] key = ac.Query[nameof(key)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                dc.Execute();
            }
            ac.GiveRedirect();
        }
    }

    public class OprPartnerOrderVarWork : OprOrderVarWork
    {
        public OprPartnerOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}