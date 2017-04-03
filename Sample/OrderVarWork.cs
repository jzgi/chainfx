using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public abstract class OrderVarWork : Work
    {
        static readonly Connector WcPay = new Connector("https://api.mch.weixin.qq.com");

        public OrderVarWork(WorkContext wc) : base(wc)
        {
        }

        public void my(ActionContext ac)
        {

        }

        ///
        ///
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[0];

            int index = 0;

            // // store backet to db
            // string openid = ac.Cookies[nameof(openid)];

            Order order = null;

            // save the order and call prepay api
            using (var dc = Service.NewDbContext())
            {
                dc.Sql("INSERT INFO orders ")._(Order.Empty)._VALUES_(Order.Empty);

                dc.Execute(p => order.WriteData(p));

                XmlContent xml = new XmlContent();
                xml.ELEM("xml", null, () =>
                {
                    xml.ELEM("appid", "");
                    xml.ELEM("mch_id", "");
                    xml.ELEM("nonce_str", "");
                    xml.ELEM("sign", "");
                    xml.ELEM("body", "");
                    xml.ELEM("out_trade_no", "");
                    xml.ELEM("total_fee", "");
                    xml.ELEM("notify_url", "");
                    xml.ELEM("trade_type", "");
                    xml.ELEM("openid", "");
                });
                var rsp = await WcPay.PostAsync("/pay/unifiedorder", xml);
                // rsp.ReadAsync<XElem>();
            }

            //  call weixin to prepay
            XmlContent cont = new XmlContent()
                .Put("out_trade_no", "")
                .Put("total_fee", 0);
            // await WCPay.PostAsync(null, "/pay/unifiedorder", cont);

        }

        public void ask(ActionContext ac)
        {
            string userid = ac[0];
            int orderid = ac[this];
            string reason = null;

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("UPDATE orders SET reason = @1, ").setstate()._(" WHERE id = @2 AND userid = @3 AND ").statecond();
                if (dc.Query(p => p.Set(reason).Set(orderid).Set(userid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
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

        [Ui(Label = "取消")]
        public void cannel(ActionContext ac)
        {
            string shopid = ac[0];
            int orderid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(orderid).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        [Ui(Label = "已备货")]
        public void fix(ActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        public void close(ActionContext ac)
        {
        }


        [Ui]
        public void exam(ActionContext ac)
        {

        }
    }

    public class MyOrderVarWork : OrderVarWork
    {
        public MyOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MyCartOrderVarWork : MyOrderVarWork
    {
        public MyCartOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MyRestOrderVarWork : MyOrderVarWork
    {
        public MyRestOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MgrOrderVarWork : OrderVarWork
    {
        public MgrOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MgrUnpaidOrderVarWork : MgrOrderVarWork
    {
        public MgrUnpaidOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MgrPaidOrderVarWork : MgrOrderVarWork
    {
        public MgrPaidOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MgrLockedOrderVarWork : MgrOrderVarWork
    {
        public MgrLockedOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MgrClosedOrderVarWork : MgrOrderVarWork
    {
        public MgrClosedOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MgrAbortedOrderVarWork : MgrOrderVarWork
    {
        public MgrAbortedOrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

}