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

        public const byte KEY = 1, LATER = 4;

        // types
        public const short POS = 1;

        public static readonly Map<short, string> Types = new Map<short, string>
        {
            {0, "普通"},
            {POS, "摊点"},
        };

        // status
        public const short CARTED = 0, PAID = 1, ABORTED = 3, FINISHED = 4;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {CARTED, "未付款"},
            {PAID, "已付款"},
            {ABORTED, "已撤单"},
            {FINISHED, "已完成"}
        };

        internal int id;
        internal short rev;
        internal short status;
        internal string shopid;
        internal string shopname;
        internal short typ; // 
        internal string wx; // weixin openid
        internal string name; // customer name
        internal string city;
        internal string addr; // may include area and site
        internal string tel;
        internal OrderItem[] items;
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

        public void Read(IDataInput i, byte proj = 0x1f)
        {
            if ((proj & KEY) == KEY)
            {
                i.Get(nameof(id), ref id);
                i.Get(nameof(rev), ref rev);
            }
            i.Get(nameof(status), ref status);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(typ), ref typ);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(name), ref name);
            i.Get(nameof(city), ref city);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(items), ref items);
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

        public void Write<R>(IDataOutput<R> o, byte proj = 0x1f) where R : IDataOutput<R>
        {
            if ((proj & KEY) == KEY)
            {
                o.Put(nameof(id), id);
                o.Put(nameof(rev), rev);
            }
            o.Put(nameof(status), status);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.Put(nameof(typ), typ);
            o.Put(nameof(wx), wx);
            o.Put(nameof(name), name);
            o.Put(nameof(city), city);
            o.Put(nameof(addr), addr);
            o.Put(nameof(tel), tel);
            o.Put(nameof(items), items);
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

        public void AddItem(string name, string unit, decimal price, short n)
        {
            int idx = items.IndexOf(o => o.name.Equals(name));
            if (idx != -1)
            {
                items[idx].qty += n;
                if (typ == POS) items[idx].load -= n; // deduce pos load
            }
            else
            {
                items = items.AddOf(new OrderItem {name = name, unit = unit, price = price, qty = n});
            }
        }

        public void UpdItem(int idx, short n)
        {
            if (typ == POS)
            {
                items[idx].load += (short) (items[idx].qty - n); // affect load
                items[idx].qty = n;
            }
            else
            {
                if (n <= 0)
                {
                    items = items.RemovedOf(idx);
                }
                else
                {
                    items[idx].qty = n;
                }
            }
        }

        public void ReceiveItem(string name, string unit, decimal price, short n)
        {
            int idx = items.IndexOf(o => o.name.Equals(name));
            if (idx != -1)
            {
                items[idx].load += n;
            }
            else
            {
                items = items.AddOf(new OrderItem {name = name, unit = unit, price = price, load = n});
            }
        }

        public void TotalUp()
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

        public static bool Deduce(OrderItem[] a, OrderItem[] b)
        {
            for (var i = 0; i < b.Length; i++)
            {
                bool match = false;
                for (var j = 0; j < a.Length; j++)
                {
                    if (a[j].name == b[i].name)
                    {
                        a[j].load -= b[i].qty;
                        if (a[j].load >= 0)
                        {
                            match = true;
                            break;
                        }
                    }
                }
                if (!match) return false;
            }
            return true;
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

        public void Read(IDataInput i, byte proj = 0x1f)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(load), ref load);
        }

        public void Write<R>(IDataOutput<R> o, byte proj = 0x1f) where R : IDataOutput<R>
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