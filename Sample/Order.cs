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
            ORD_ASSIGNED = 2,
            ORD_SUPPLIED = 3,
            ORD_SHIPPED = 5,
            ORD_ENDED = 7;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {ORD_ABORTED, "已撤"},
            {ORD_CREATED, null},
            {ORD_PAID, "新收"},
            {ORD_ASSIGNED, "已排"},
            {ORD_SUPPLIED, "已备"},
            {ORD_SHIPPED, "已送"},
            {ORD_ENDED, "完成"}
        };

        internal int id;
        internal int uid;
        internal string uname; // customer name
        internal string uwx; // weixin openid
        internal string utel;
        internal string uaddr; // may include area and site
        internal string teamid;
        internal string item;
        internal string unit;
        internal decimal price;
        internal short qty;
        internal decimal total; // total price
        internal decimal cash; // cash paid
        internal int score; // deduction of points
        internal DateTime paid;
        internal string shopid; // shop org id
        internal DateTime assigned;
        internal DateTime supplied;
        internal int shipuid;
        internal DateTime shipped;
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
            s.Get(nameof(teamid), ref teamid);
            s.Get(nameof(item), ref item);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);
            s.Get(nameof(total), ref total);
            s.Get(nameof(cash), ref cash);
            s.Get(nameof(score), ref score);
            s.Get(nameof(paid), ref paid);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(shopid), ref shopid);
                s.Get(nameof(assigned), ref assigned);
                s.Get(nameof(supplied), ref supplied);
                s.Get(nameof(shipuid), ref shipuid);
                s.Get(nameof(shipped), ref shipped);
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
            s.Put(nameof(teamid), teamid);
            s.Put(nameof(item), item);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);
            s.Put(nameof(total), total);
            s.Put(nameof(cash), cash);
            s.Put(nameof(score), score);
            s.Put(nameof(paid), paid);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(shopid), shopid);
                s.Put(nameof(assigned), assigned);
                s.Put(nameof(supplied), supplied);
                s.Put(nameof(shipuid), shipuid);
                s.Put(nameof(shipped), shipped);
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