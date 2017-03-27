using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public class CartFolder : Folder
    {
        static readonly Connector WcPay = new Connector("https://api.mch.weixin.qq.com");

        // keyed by wx
        readonly ConcurrentDictionary<string, Cart> carts;

        public CartFolder(FolderContext fc) : base(fc)
        {
            carts = new ConcurrentDictionary<string, Cart>(8, 1024);
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[typeof(UserVarFolder)];
            Cart cart;
            if (carts.TryGetValue(wx, out cart))
            {

                // ac.GivePage(200, )

            }
        }

        ///
        /// <param name="shopid"/>
        /// <param name="item"/>
        /// <param name="qty"/>
        ///
        public async Task add(ActionContext ac)
        {
            string wx = ac[typeof(UserVarFolder)];
            var ln = await ac.ReadObjectAsync<OrderLine>();

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT price FROM items WHERE shopid = @1 AND item = @2", p => p.Set(ln.shopid).Set(ln.item)))
                {
                    dc.Get(null, ref ln.price);
                }
            }

            // add or merge into cart
            Cart cart = carts.GetOrAdd(wx, (key) => new Cart(8));
            lock (cart)
            {
                int exist = -1;
                for (int i = 0; i < cart.Count; i++)
                {
                    // Order cartln = cart[i];
                    // if (cartln.shopid.Equals(ln.shopid) && cartln.item.Equals(ln.item))
                    // {
                    //     exist = i;
                    //     break;
                    // }
                }

                if (ln.qty <= 0) // remove a line
                {
                    if (exist != -1) cart.RemoveAt(exist);
                }
                else // add or merge
                {
                    if (exist != -1)
                    {
                        // cart[exist].AddQty(ln.qty); // merge
                    }
                    else
                    {
                        // cart.Add(ln); //add the line
                    }
                }
            }

            ac.Give(200);
        }

        ///
        ///
        [User]
        public void empty(ActionContext ac)
        {
            string wx = ac[1];

            Cart cart;
            if (carts.TryGetValue(wx, out cart))
            {
                lock (cart)
                {
                    cart.Clear();
                }
            }
            ac.Give(200);
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