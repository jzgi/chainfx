using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// An order data object.
    /// </summary>
    public class Ord : IData
    {
        public static readonly Ord Empty = new Ord();

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
        internal string ctrid; // center id
        internal string grpid; // team id
        internal string supid; // supply

        internal int uid;
        internal string uname; // customer name
        internal string uwx; // weixin openid
        internal string utel;
        internal string uaddr; // may include area and site

        internal string item;
        internal string unit;
        internal decimal price;
        internal short qty;
        internal decimal total; // total price
        internal decimal cash; // cash paid
        internal int score; // deduction of points

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
            s.Get(nameof(ctrid), ref ctrid);
            s.Get(nameof(grpid), ref grpid);
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(uwx), ref uwx);
            s.Get(nameof(utel), ref utel);
            s.Get(nameof(uaddr), ref uaddr);

            s.Get(nameof(item), ref item);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);

            s.Get(nameof(total), ref total);
            s.Get(nameof(score), ref score);
            s.Get(nameof(created), ref created);
            if ((proj & LATER) == LATER)
            {
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
            s.Put(nameof(ctrid), ctrid);
            s.Put(nameof(grpid), grpid);
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(uwx), uwx);
            s.Put(nameof(utel), utel);
            s.Put(nameof(uaddr), uaddr);

            s.Put(nameof(item), item);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);

            s.Put(nameof(total), total);
            s.Put(nameof(score), score);
            s.Put(nameof(created), created);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(cash), cash);
                s.Put(nameof(paid), paid);
                s.Put(nameof(ended), ended);
            }
            s.Put(nameof(status), status);
        }

        public string Err()
        {
            if (uaddr == null) return "您尚未填写地址哦！";
            return null;
        }
    }
}