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

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & KEY) == KEY)
            {
                s.Get(nameof(id), ref id);
                s.Get(nameof(rev), ref rev);
            }
            s.Get(nameof(status), ref status);
            s.Get(nameof(shopid), ref shopid);
            s.Get(nameof(shopname), ref shopname);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(wx), ref wx);
            s.Get(nameof(name), ref name);
            s.Get(nameof(city), ref city);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(items), ref items);
            s.Get(nameof(min), ref min);
            s.Get(nameof(notch), ref notch);
            s.Get(nameof(off), ref off);
            s.Get(nameof(total), ref total);
            s.Get(nameof(created), ref created);
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(cash), ref cash);
                s.Get(nameof(paid), ref paid);
                s.Get(nameof(aborted), ref aborted);
                s.Get(nameof(finished), ref finished);
                s.Get(nameof(kick), ref kick);
            }
        }

        public void Write<R>(ISink<R> s, byte proj = 0x0f) where R : ISink<R>
        {
            if ((proj & KEY) == KEY)
            {
                s.Put(nameof(id), id);
                s.Put(nameof(rev), rev);
            }
            s.Put(nameof(status), status);
            s.Put(nameof(shopid), shopid);
            s.Put(nameof(shopname), shopname);
            s.Put(nameof(typ), typ);
            s.Put(nameof(wx), wx);
            s.Put(nameof(name), name);
            s.Put(nameof(city), city);
            s.Put(nameof(addr), addr);
            s.Put(nameof(tel), tel);
            s.Put(nameof(items), items);
            s.Put(nameof(min), min);
            s.Put(nameof(notch), notch);
            s.Put(nameof(off), off);
            s.Put(nameof(total), total);
            s.Put(nameof(created), created);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(cash), cash);
                s.Put(nameof(paid), paid);
                s.Put(nameof(aborted), aborted);
                s.Put(nameof(finished), finished);
                s.Put(nameof(kick), kick);
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

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(name), ref name);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);
            s.Get(nameof(load), ref load);
        }

        public void Write<R>(ISink<R> s, byte proj = 0x0f) where R : ISink<R>
        {
            s.Put(nameof(name), name);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);
            s.Put(nameof(load), load);
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}