using System.Collections.Concurrent;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// A cart pertaining to one user. (placed under UserVar)
    ///
    public class CartWork : Work
    {
        static readonly Connector WcPay = new Connector("https://api.mch.weixin.qq.com");

        // keyed by wx
        internal readonly ConcurrentDictionary<string, Cart> carts;

        public CartWork(WorkContext fc) : base(fc)
        {
            CreateVar<CartOrderWork>();

            carts = new ConcurrentDictionary<string, Cart>(8, 1024);
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[typeof(UserWork)];
            Cart cart;
            if (carts.TryGetValue(wx, out cart))
            {

                ac.GiveWorkPage(200, cart, 0);

            }
        }

        ///
        /// <param name="shopid"/>
        /// <param name="item"/>
        /// <param name="qty"/>
        ///
        public async Task add(ActionContext ac)
        {
            string wx = ac[typeof(UserWork)];
            var ln = await ac.ReadObjectAsync<OrderLine>();

            using (var dc = ac.NewDbContext())
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
    }
}