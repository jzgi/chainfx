using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// An order data object.
    /// </summary>
    public class Order : IData
    {
        public static readonly Order Empty = new Order();

        public const byte KEY = 1, DETAIL = 2, LATER = 4;

        // types
        public const short POS = 1;

        // status
        public const short ABORTED = -1, CREATED = 0, PAID = 1, ENDED = 2;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {ABORTED, "已撤单"},
            {CREATED, "未付款"},
            {PAID, "已付款"},
            {ENDED, "已完成"}
        };

        internal int id;
        internal short rev;
        internal string orgid;
        internal string orgname;
        internal int custid;
        internal string custname; // customer name
        internal string custwx; // weixin openid
        internal string custtel;
        internal string custaddr; // may include area and site
        internal OrderItem[] items;
        internal decimal total; // total price
        internal decimal cash; // cash payment
        internal decimal points; // deduction of points
        internal int posid; // POS id
        internal DateTime created;
        internal DateTime paid;
        internal DateTime ended;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & KEY) == KEY)
            {
                s.Get(nameof(id), ref id);
                s.Get(nameof(rev), ref rev);
            }
            s.Get(nameof(orgid), ref orgid);
            s.Get(nameof(orgname), ref orgname);
            s.Get(nameof(custid), ref custid);
            s.Get(nameof(custname), ref custname);
            s.Get(nameof(custwx), ref custwx);
            s.Get(nameof(custtel), ref custtel);
            s.Get(nameof(custaddr), ref custaddr);
            if ((proj & DETAIL) == DETAIL)
            {
                s.Get(nameof(items), ref items);
            }
            s.Get(nameof(total), ref total);
            s.Get(nameof(points), ref points);
            s.Get(nameof(created), ref created);
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(posid), ref posid);
                s.Get(nameof(cash), ref cash);
                s.Get(nameof(paid), ref paid);
                s.Get(nameof(ended), ref ended);
            }
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & KEY) == KEY)
            {
                s.Put(nameof(id), id);
                s.Put(nameof(rev), rev);
            }
            s.Put(nameof(orgid), orgid);
            s.Put(nameof(orgname), orgname);
            s.Put(nameof(custid), custid);
            s.Put(nameof(custname), custname);
            s.Put(nameof(custwx), custwx);
            s.Put(nameof(custtel), custtel);
            s.Put(nameof(custaddr), custaddr);
            if ((proj & DETAIL) == DETAIL)
            {
                s.Put(nameof(items), items);
            }
            s.Put(nameof(total), total);
            s.Put(nameof(points), points);
            s.Put(nameof(created), created);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(posid), posid);
                s.Put(nameof(cash), cash);
                s.Put(nameof(paid), paid);
                s.Put(nameof(ended), ended);
            }
            s.Put(nameof(status), status);
        }

        public string Err()
        {
            if (custaddr == null) return "您尚未填写地址哦！";
            return null;
        }

        public void AddItem(string name, string unit, decimal price, decimal comp, short num)
        {
            int idx = items.IndexOf(o => o.name.Equals(name));
            if (idx != -1)
            {
                items[idx].qty += num;
            }
            else
            {
                items = items.AddOf(new OrderItem
                {
                    name = name,
                    unit = unit,
                    price = price,
                    qty = num
                });
            }
            Calc();
        }

        public void UpdItem(int idx, short num)
        {
            items[idx].qty = num;
            if (num <= 0)
            {
                items = items.RemovedOf(idx);
            }
            else
            {
                items[idx].qty = num;
            }
            Calc();
        }

        // calculate total price and net price
        public void Calc()
        {
            decimal sum = 0;
            decimal deduct = 0;
            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    sum += items[i].qty * items[i].price;
                }
            }
            total = sum;
            points = sum - deduct;
        }
    }

    public struct OrderItem : IData
    {
        internal string name;
        internal string unit;
        internal decimal price;
        internal short qty;

        public decimal Subtotal => price * qty;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(name), ref name);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(name), name);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);
        }
    }
}