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
            OrdAborted = -1,
            OrdCreated = 0,
            OrdPaid = 1,
            OrdProvided = 3,
            OrdShipped = 5,
            OrdEnded = 7;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {OrdAborted, "已撤"},
            {OrdCreated, null},
            {OrdPaid, "新收"},
            {OrdProvided, "已备"},
            {OrdShipped, "已送"},
            {OrdEnded, "完成"}
        };

        internal int id;
        internal int uid;
        internal string uname; // customer name
        internal string uwx; // weixin openid
        internal string utel;
        internal string uaddr; // may include area and site
        internal string teamid;
        internal short itemid;
        internal string itemname;
        internal string unit;
        internal decimal price;
        internal short qty;
        internal decimal total; // total price
        internal decimal cash; // cash paid
        internal DateTime paidon;
        internal short shopid; // workshop's orgid
        internal int shipperid; // shipper's userid, 
        internal DateTime shippedon;
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
            s.Get(nameof(itemid), ref itemid);
            s.Get(nameof(itemname), ref itemname);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);
            s.Get(nameof(total), ref total);
            s.Get(nameof(cash), ref cash);
            s.Get(nameof(paidon), ref paidon);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(shopid), ref shopid);
                s.Get(nameof(shipperid), ref shipperid);
                s.Get(nameof(shippedon), ref shippedon);
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
            s.Put(nameof(itemid), itemid);
            s.Put(nameof(itemname), itemname);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);
            s.Put(nameof(total), total);
            s.Put(nameof(cash), cash);
            s.Put(nameof(paidon), paidon);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(shopid), shopid);
                s.Put(nameof(shipperid), shipperid);
                s.Put(nameof(shippedon), shippedon);
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