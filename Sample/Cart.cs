using System.Collections.Generic;

namespace Greatbone.Sample
{
    /// 
    /// A shopping cart contains a number of pre-orders.
    ///
    public class Cart : List<Order>
    {
        public Cart(int capacity) : base(capacity) { }

        public void add(string shopid, string item, short qty)
        {
            var order = Find(o => o.shopid.Equals(shopid));
            if (order == null)
            {
                order = new Order();
                Add(order);
            }
        }
    }
}