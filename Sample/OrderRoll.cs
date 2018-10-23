using Greatbone;

namespace Samp
{
    public class OrderRoll : Roll<short, OrderAgg>
    {
        int qty;

        string[] oprs;

        public OrderRoll()
        {
        }

        protected internal override void Add(OrderAgg v)
        {
            base.Add(v);

            qty += v.qty;
            oprs = oprs.MergeOf(v.oprs);
        }

        public string[] Oprs => oprs;
    }
}