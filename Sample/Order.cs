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
            ID = 1,
            LATER = 4;

        // status
        public const short CREATED = 0, PAID = 1, READY = 2, ABORTED = 3, DONE = 4;

        // status
        public static readonly Map<short, string> STATUS = new Map<short, string>
        {
            {CREATED, "购物车"},
            {PAID, "已付款"},
            {READY, "已备货"},
            {ABORTED, "已撤单"},
            {DONE, "已送达"}
        };


        internal int id;
        internal short rev;
        internal string shopid;
        internal string shopname;
        internal string wx; // weixin openid
        internal string name; // customer name
        internal string tel;
        internal string city;
        internal string area;
        internal string addr; // may include spot
        internal OrderItem[] items;
        internal decimal min;
        internal decimal notch;
        internal decimal off;
        internal decimal total; // total price

        internal decimal cash; // amount recieved
        internal DateTime paid; // when cash received or forcibly accepted
        internal bool prepare;
        internal DateTime aborted; // time aborted
        internal DateTime received; // time shipped
        internal string note;
        internal short status;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
                i.Get(nameof(rev), ref rev);
            }
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(area), ref area);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(items), ref items);
            i.Get(nameof(min), ref min);
            i.Get(nameof(notch), ref notch);
            i.Get(nameof(off), ref off);
            i.Get(nameof(total), ref total);
            if ((proj & LATER) == LATER)
            {
                i.Get(nameof(cash), ref cash);
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(prepare), ref prepare);
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
                o.Put(nameof(rev), rev);
            }
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.Put(nameof(wx), wx);
            o.Put(nameof(name), name);
            o.Put(nameof(tel), tel);
            o.Put(nameof(city), city);
            o.Put(nameof(area), area);
            o.Put(nameof(addr), addr);
            o.Put(nameof(items), items);
            o.Put(nameof(min), min);
            o.Put(nameof(notch), notch);
            o.Put(nameof(off), off);
            o.Put(nameof(total), total);
            if ((proj & LATER) == LATER)
            {
                o.Put(nameof(cash), cash);
                o.Put(nameof(paid), paid);
                o.Put(nameof(prepare), prepare);
                o.Put(nameof(aborted), aborted);
                o.Put(nameof(received), received);
                o.Put(nameof(note), note);
            }
            o.Put(nameof(status), status);
        }

        public string Err()
        {
            if (total < min) return "不足最低金额，请继续选购！";
            if (addr == null) return "您尚未填写地址哦！";
            return null;
        }

        public void AddItem(string name, decimal price, short qty, string unit, string[] customs)
        {
            if (items == null)
            {
                items = new[] {new OrderItem {name = name, price = price, qty = qty, unit = unit, opts = customs}};
            }
            int idx = items.FindIndex(o => o.name.Equals(name) && o.opts.SameAs(customs));
            if (idx != -1)
            {
                items[idx].qty += qty;
            }
            else
            {
                items = items.AddOf(new OrderItem() {name = name, qty = qty, unit = unit, price = price, opts = customs});
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
                if (notch > 0)
                {
                    total = total - (decimal.Floor(total / notch) * off);
                }
            }
        }

        public void UpdItem(int idx, short qty, string[] opts)
        {
            if (qty <= 0)
            {
                items = items.RemovedOf(idx);
            }
            else
            {
                items[idx].qty = qty;
                items[idx].opts = opts;
            }
        }
    }

    public struct OrderItem : IData
    {
        internal string name;
        internal decimal price;
        internal short qty;
        internal string unit;
        internal string[] opts;

        public decimal Subtotal => price * qty;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(price), ref price);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(opts), ref opts);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(price), price);
            o.Put(nameof(qty), qty);
            o.Put(nameof(unit), unit);
            o.Put(nameof(opts), opts);
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}