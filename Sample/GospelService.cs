using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    public class GospelService : Service, ICatch
    {
        public GospelService(ServiceContext sc) : base(sc)
        {
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            if (e is AuthorizeException)
            {
                if (ac.Principal == null)
                {
                    // weixin authorization challenge
                    if (ac.ByWeiXin) // weixin
                    {
                        ac.GiveRedirectWeiXinAuthorize();
                    }
                    else // challenge BASIC scheme
                    {
                        ac.SetHeader("WWW-Authenticate", "Basic realm=\"APP\"");
                        ac.Give(401); // unauthorized
                    }
                }
                else
                {
                    ac.GivePage(403, m => { m.CALLOUT("您目前没有访问权限!"); });
                }
            }
            else
            {
                ac.Give(500, e.Message);
            }
        }

        public void @default(ActionContext ac)
        {
            DoFile("default.html", ".html", ac);
        }

        /// <summary>
        /// WCPay notify, placed here due to non-authentic context.
        /// </summary>
        public async Task notify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();

            long orderid;
            decimal cash;
            if (Notified(xe, out orderid, out cash))
            {
                string mgrwx = null;
                using (var dc = NewDbContext())
                {
                    var shopid = (string) dc.Scalar("UPDATE orders SET cash = @1, accepted = localtimestamp, status = @2 WHERE id = @3 AND status <= @2 RETURNING shopid", (p) => p.Set(cash).Set(Order.ACCEPTED).Set(orderid));
                    if (shopid != null)
                    {
                        mgrwx = (string) dc.Scalar("SELECT mgrwx FROM shops WHERE id = @1", p => p.Set(shopid));
                    }
                }
                // return xml
                XmlContent cont = new XmlContent(true, 1024);
                cont.ELEM("xml", null, () =>
                {
                    cont.ELEM("return_code", "SUCCESS");
                    cont.ELEM("return_msg", "OK");
                });

                if (mgrwx != null)
                {
                    await PostSendAsync(mgrwx, "【买家付款】订单编号：" + orderid + "，金额：" + cash + "元");
                }

                ac.Give(200, cont);
            }
            else
            {
                ac.Give(400);
            }
        }
    }
}