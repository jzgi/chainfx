using Greatbone.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Greatbone.Sample
{
    ///
    ///
    public class CartVarFolder : Folder, IVar
    {
        // all carts keyed by userid
        readonly ConcurrentDictionary<string, Cart> carts;

        public CartVarFolder(FolderContext dc) : base(dc)
        {
        }

        public void @default(ActionContext ac)
        {
            ac.ReplyFolderPage(200, (List<Item>)null);
        }

        ///
        /// <param name="shopid"/>
        /// <param name="item"/>
        /// <param name="qty"/>
        ///
        public async Task add(ActionContext ac)
        {
            string wx = ac[0];
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

            ac.Reply(200);
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
            ac.Reply(200);
        }

        ///
        ///
        public void checkout(ActionContext ac)
        {
            string wx = ac[0];

            if (ac.GET)
            { // give change to review the orders

                List<Order> orders = GetOrderList();
                ac.ReplyJson(200, orders);
            }
            else
            {
                // // store backet to db
                // string openid = ac.Cookies[nameof(openid)];

                List<Order> orders = GetOrderList();

                // save the orders to db
                using (var dc = Service.NewDbContext())
                {
                    dc.Sql("INSERT INFO orders ")._(Order.Empty)._VALUES_(Order.Empty);

                    foreach (var order in orders)
                    {
                        dc.Execute(p => order.WriteData(p));
                    }

                    // remove cart 
                    Cart cart;
                    carts.TryRemove(wx, out cart);
                }

                //  call weixin to prepay
                XmlContent cont = new XmlContent()
                    .Put("out_trade_no", "")
                    .Put("total_fee", 0);
                // await WCPay.PostAsync(null, "/pay/unifiedorder", cont);

            }
        }

        List<Order> GetOrderList()
        {
            return null;
        }
    }
}