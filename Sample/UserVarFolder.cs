using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public class UserVarFolder : WebFolder, IVar
    {
        readonly ConcurrentDictionary<string, List<OrderLine>> carts;

        public UserVarFolder(WebFolderContext dc) : base(dc)
        {
            Create<OrderFolder>("order");

            carts = new ConcurrentDictionary<string, List<OrderLine>>(8, 1024);
        }

        ///
        /// POST /buyer/-id-/cart
        ///
        /// shopid=_id_&amp;item=_item_&amp;qty=_n_
        ///
        public async Task cart(WebActionContext ac)
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
            List<OrderLine> cart = carts.GetOrAdd(wx, (key) => new List<OrderLine>(8));
            lock (cart)
            {
                int exist = -1;
                for (int i = 0; i < cart.Count; i++)
                {
                    OrderLine cartln = cart[i];
                    if (cartln.shopid.Equals(ln.shopid) && cartln.item.Equals(ln.item))
                    {
                        exist = i;
                        break;
                    }
                }

                if (ln.qty <= 0) // remove a line
                {
                    if (exist != -1) cart.RemoveAt(exist);
                }
                else // add or merge
                {
                    if (exist != -1)
                    {
                        cart[exist].AddQty(ln.qty); // merge
                    }
                    else
                    {
                        cart.Add(ln); //add the line
                    }
                }
            }

            ac.Reply(200);
        }

        ///
        /// POST /buyer/-id-/empty
        ///
        /// shopid=_id_&amp;item=_item_&amp;qty=_n_
        ///
        public void empty(WebActionContext ac)
        {
            string wx = ac[0];

            // TODO access check 

            List<OrderLine> cart;
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
        /// GET /buyer/-wx-/checkout
        ///
        /// POST /buyer/-wx-/checkout
        ///
        public void checkout(WebActionContext ac)
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
                    List<OrderLine> cart;
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

        ///
        ///
        /// GET /buyer/-wx-/my
        ///
        public void my(WebActionContext ac)
        {
            string wx = ac[0];

            // TODO check access

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE buyerwx = @1 AND status > 0", p => p.Set(wx)))
                {
                    ac.Reply(200, dc.Dump<JsonContent>());
                }
                else
                {
                    ac.Reply(204);
                }
            }
        }
    }
}