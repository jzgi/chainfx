using System;
using Greatbone.Core;

namespace Greatbone.Samp
{
    /// <summary>
    /// An order data object.
    /// </summary>
    public class Order : IData
    {
        public static readonly Order Empty = new Order();

        public const short ID = 1, LATER = 4;

        // status
        public const short CART = 0, PAID = 1, ABORTED = 3, FINISHED = 4;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {CART, "购物车"},
            {PAID, "已付款"},
            {ABORTED, "已撤单"},
            {FINISHED, "已完成"}
        };

        internal int id;
        internal short rev;
        internal short status;
        internal string shopid;
        internal string shopname;
        internal bool pos; // whether an onsite sales order
        internal string wx; // weixin openid
        internal string name; // customer name
        internal string tel;
        internal string city;
        internal string addr; // may include area and site
        internal OrderItem[] items;
        internal string note; // comment
        internal decimal min;
        internal decimal notch;
        internal decimal off;
        internal decimal total; // total price
        internal DateTime created;
        internal decimal cash; // amount recieved
        internal DateTime paid;
        internal DateTime aborted;
        internal DateTime finished;
        internal string kick;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
                i.Get(nameof(rev), ref rev);
            }
            i.Get(nameof(status), ref status);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(pos), ref pos);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(name), ref name);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(items), ref items);
            i.Get(nameof(note), ref note);
            i.Get(nameof(min), ref min);
            i.Get(nameof(notch), ref notch);
            i.Get(nameof(off), ref off);
            i.Get(nameof(total), ref total);
            i.Get(nameof(created), ref created);
            if ((proj & LATER) == LATER)
            {
                i.Get(nameof(cash), ref cash);
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(aborted), ref aborted);
                i.Get(nameof(finished), ref finished);
                i.Get(nameof(kick), ref kick);
            }
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id);
                o.Put(nameof(rev), rev);
            }
            o.Put(nameof(status), status);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.Put(nameof(pos), pos);
            o.Put(nameof(wx), wx);
            o.Put(nameof(name), name);
            o.Put(nameof(tel), tel);
            o.Put(nameof(city), city);
            o.Put(nameof(addr), addr);
            o.Put(nameof(items), items);
            o.Put(nameof(note), note);
            o.Put(nameof(min), min);
            o.Put(nameof(notch), notch);
            o.Put(nameof(off), off);
            o.Put(nameof(total), total);
            o.Put(nameof(created), created);
            if ((proj & LATER) == LATER)
            {
                o.Put(nameof(cash), cash);
                o.Put(nameof(paid), paid);
                o.Put(nameof(aborted), aborted);
                o.Put(nameof(finished), finished);
                o.Put(nameof(kick), kick);
            }
        }

        public string Err()
        {
            if (total < min) return "不足最低金额，请继续选购！";
            if (addr == null) return "您尚未填写地址哦！";
            return null;
        }

        public void AddItem(string name, string unit, decimal price, short num)
        {
            int idx = items.FindIndex(o => o.name.Equals(name));
            if (idx != -1)
            {
                if (pos) items[idx].load += num;
                else items[idx].qty += num;
            }
            else
            {
                var o = new OrderItem {name = name, unit = unit, price = price, qty = num};
                if (pos) o.load = num;
                else o.qty = num;
                items = items.AddOf(o);
            }
        }

        public void UpdItem(string name, short qty)
        {
            int idx = items.FindIndex(o => o.name.Equals(name));
            if (qty <= 0)
            {
                items = items.RemovedOf(idx);
            }
            else
            {
                items[idx].qty = qty;
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
    }

    public struct OrderItem : IData
    {
        internal string name;
        internal string unit;
        internal decimal price;
        internal short qty;
        internal short load; // work order kept

        public decimal Subtotal => price * qty;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(load), ref load);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(unit), unit);
            o.Put(nameof(price), price);
            o.Put(nameof(qty), qty);
            o.Put(nameof(load), load);
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}