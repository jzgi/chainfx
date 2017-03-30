
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// An order item in cart.
    ///
    public class CartVarVarWork : Work
    {
        IDictionary<string, Cart> carts;

        public CartVarVarWork(WorkContext fc) : base(fc)
        {
        }

        public void edit(ActionContext ac)
        {
            string wx = ac[-2];
            Cart cart = carts[wx];
            lock (cart)
            {
                string shopid = ac[-1];
                int orderidx = cart.FindIndex(x => x.shopid.Equals(shopid));
                Order order = cart[orderidx];
                int lineidx = ac[this];
                List<OrderLine> lines = order.lines;
                if (lines != null)
                {
                    OrderLine line = lines[lineidx];
                    if (ac.GET)
                    {

                    }
                    else
                    {

                    }
                }
            }
        }

        public void rm(ActionContext ac)
        {
            string wx = ac[-2];
            Cart cart = carts[wx];
            lock (cart)
            {
                string shopid = ac[-1];
                int orderidx = cart.FindIndex(x => x.shopid.Equals(shopid));
                Order order = cart[orderidx];
                int index = ac[this];
                List<OrderLine> lines = order.lines;
                if (lines != null)
                {
                    lines.RemoveAt(index);
                    if (lines.Count == 0)
                    {
                        cart.RemoveAt(orderidx);
                        if (cart.Count == 0)
                        {
                            carts.Remove(wx);
                        }
                    }
                }
            }
        }
    }
}