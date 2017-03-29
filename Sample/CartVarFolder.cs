using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// An order in cart
    ///
    public class CartVarFolder : Folder, IVar
    {
        static readonly Connector WcPay = new Connector("https://api.mch.weixin.qq.com");

        // keyed by wx
        internal readonly ConcurrentDictionary<string, Cart> carts;

        public CartVarFolder(FolderContext fc) : base(fc)
        {
            CreateVar<CartItemFolder>();

            carts = new ConcurrentDictionary<string, Cart>(8, 1024);
        }

        public void pay(ActionContext ac)
        {
            string wx = ac[-1];
            Cart cart = carts[wx];
            string shopid = ac[this];

            Order order = cart.Find(x => x.shopid.Equals(shopid));


        }

        ///
        ///
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[0];

            int index = 0;

            // // store backet to db
            // string openid = ac.Cookies[nameof(openid)];

            Cart cart;
            if (!carts.TryGetValue(wx, out cart))
            {

            }
            var order = cart[index];

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
    }
}