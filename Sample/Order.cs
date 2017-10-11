using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// An order data object.
    /// </summary>
    public class Order : IData
    {
        public static readonly Order Empty = new Order();

        public const short
            ID = 0x0001,
            BASIC = 0x0002,
            LATER = 0x0200;

        // status
        public const short CREATED = 0, PAID = 1, ABORTED = 2, READY = 3, RECEIVED = 4, RECKONED = 5;

        // status
        public static readonly Map<short, string> STATUS = new Map<short, string>
        {
            [CREATED] = "购物车",
            [PAID] = "已付款",
            [ABORTED] = "已撤单",
            [READY] = "已备货",
            [RECEIVED] = "已确收",
            [RECKONED] = "已清算",
        };


        internal int id;
        internal short shopid;
        internal string shopname;
        internal string wx; // weixin openid
        internal string name; // customer name
        internal string tel;
        internal string city;
        internal string region; // distr or area
        internal string addr; // address
        internal OrderItem[] items;
        internal decimal total; // receivable
        internal DateTime created; // time created
        internal decimal cash; // amount recieved
        internal DateTime paid; // when cash received or forcibly accepted
        internal DateTime aborted; // time aborted
        internal DateTime received; // time shipped
        internal string note;
        internal short status;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(region), ref region);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(items), ref items);
            i.Get(nameof(total), ref total);
            i.Get(nameof(created), ref created);
            if ((proj & LATER) == LATER)
            {
                i.Get(nameof(cash), ref cash);
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(aborted), ref aborted);
                i.Get(nameof(received), ref received);
                i.Get(nameof(note), ref note);
            }
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id);
            }
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.Put(nameof(wx), wx);
            o.Put(nameof(name), name);
            o.Put(nameof(tel), tel);
            o.Put(nameof(city), city);
            o.Put(nameof(region), region);
            o.Put(nameof(addr), addr);
            o.Put(nameof(items), items);
            o.Put(nameof(total), total);
            o.Put(nameof(created), created);
            if ((proj & LATER) == LATER)
            {
                o.Put(nameof(cash), cash);
                o.Put(nameof(paid), paid);
                o.Put(nameof(aborted), aborted);
                o.Put(nameof(received), received);
                o.Put(nameof(note), note);
            }
            o.Put(nameof(status), status);
        }

        public void AddItem(string name, decimal price, short qty, string unit, string[] customs)
        {
            if (items == null)
            {
                items = new[] {new OrderItem {name = name, price = price, qty = qty, unit = unit, customs = customs}};
            }
            var orderitem = items.Find(o => o.name.Equals(name) && o.customs.SameAs(customs));
            if (orderitem.name != null)
            {
                orderitem.qty += qty;
            }
            else
            {
                items = items.AddOf(new OrderItem() {name = name, qty = qty, unit = unit, price = price, customs = customs});
            }
        }

        public void SetTotal()
        {
            if (items != null)
            {
                decimal sum = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    sum += items[i].qty * items[i].price;
                }
                total = sum;
            }
        }

        public void SetItemQty(int idx, short qty)
        {
            var orderitem = items[idx];
            orderitem.qty = qty;
        }

        public void RemoveDetail(string name)
        {
            items = items.RemovedOf(x => x.name == name);
        }
    }

    public struct OrderItem : IData
    {
        internal string name;
        internal decimal price;
        internal short qty;
        internal string unit;
        internal string[] customs;

        public decimal Subtotal => price * qty;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(price), ref price);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(customs), ref customs);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(price), price);
            o.Put(nameof(qty), qty);
            o.Put(nameof(unit), unit);
            o.Put(nameof(customs), customs);
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}