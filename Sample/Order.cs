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

        public const byte KEY = 1, LATER = 4;

        // status
        public const short
            ORD_ABORTED = -1,
            ORD_CREATED = 0,
            ORD_PAID = 1,
            ORD_GIVING = 2,
            ORD_GIVEN = 3,
            ORD_DVRING = 4,
            ORD_DVRED = 5,
            ORD_ENDED = 7;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {ORD_ABORTED, "已撤销"},
            {ORD_CREATED, "新创建"},
            {ORD_PAID, "已付款"},
            {ORD_GIVING, "排程中"},
            {ORD_GIVEN, "已备齐"},
            {ORD_DVRING, "派送中"},
            {ORD_DVRED, "已送达"},
            {ORD_ENDED, "已完成"}
        };

        internal int id;
        internal int uid;
        internal string uname; // customer name
        internal string uwx; // weixin openid
        internal string utel;
        internal string uaddr; // may include area and site
        internal string grpid;
        internal string item;
        internal string unit;
        internal decimal price;
        internal short qty;
        internal decimal total; // total price
        internal decimal cash; // cash paid
        internal int score; // deduction of points
        internal DateTime created;

        internal DateTime paid;
        internal DateTime aborted;
        internal int giverid;
        internal DateTime giving;
        internal DateTime given;
        internal int dvrerid;
        internal DateTime dvring;
        internal DateTime dvred;
        internal int grperid;
        internal DateTime ended;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & KEY) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(uwx), ref uwx);
            s.Get(nameof(utel), ref utel);
            s.Get(nameof(uaddr), ref uaddr);
            s.Get(nameof(grpid), ref grpid);
            s.Get(nameof(item), ref item);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);
            s.Get(nameof(total), ref total);
            s.Get(nameof(cash), ref cash);
            s.Get(nameof(score), ref score);
            s.Get(nameof(created), ref created);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(aborted), ref aborted);
                s.Get(nameof(paid), ref paid);
                s.Get(nameof(giverid), ref giverid);
                s.Get(nameof(giving), ref giving);
                s.Get(nameof(given), ref given);
                s.Get(nameof(dvrerid), ref dvrerid);
                s.Get(nameof(dvring), ref dvring);
                s.Get(nameof(dvred), ref dvred);
                s.Get(nameof(grperid), ref grperid);
                s.Get(nameof(ended), ref ended);
            }
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & KEY) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(uwx), uwx);
            s.Put(nameof(utel), utel);
            s.Put(nameof(uaddr), uaddr);
            s.Put(nameof(grpid), grpid);
            s.Put(nameof(item), item);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);
            s.Put(nameof(total), total);
            s.Put(nameof(cash), cash);
            s.Put(nameof(score), score);
            s.Put(nameof(created), created);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(aborted), aborted);
                s.Put(nameof(paid), paid);
                s.Put(nameof(giverid), giverid);
                s.Put(nameof(giving), giving);
                s.Put(nameof(given), given);
                s.Put(nameof(dvrerid), dvrerid);
                s.Put(nameof(dvring), dvring);
                s.Put(nameof(dvred), dvred);
                s.Put(nameof(grperid), grperid);
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