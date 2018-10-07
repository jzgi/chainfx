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

        public const byte ID = 1, LATER = 4;

        // status
        public const short
            OrdAborted = -1, OrdCreated = 0, OrdPaid = 1, OrdAccepted = 2, OrdTaken = 3, OrdSent = 4, OrdReceived = 5, OrdEnded = 6;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {OrdAborted, "已撤销"},
            {OrdCreated, null},
            {OrdPaid, "待备货"},
            {OrdAccepted, "备货中"},
            {OrdTaken, "中转站"},
            {OrdSent, "运送中"},
            {OrdReceived, "已运达"},
            {OrdEnded, "完成"}
        };

        internal int id;
        internal string hubid;
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
        internal DateTime paid;
        internal short shopid; // workshop's orgid
        internal int accepterid; // giving forth goods
        internal DateTime accepted;
        internal int takerid; // taking goods from the giver to the storage 
        internal DateTime taken;
        internal int senderid; // sending goods to the team 
        internal DateTime sent;
        internal int receiverid; // receiving goods for the team 
        internal DateTime received;
        internal DateTime ended;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(hubid), ref hubid);
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
            s.Get(nameof(paid), ref paid);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(shopid), ref shopid);
                s.Get(nameof(accepterid), ref accepterid);
                s.Get(nameof(accepted), ref accepted);
                s.Get(nameof(takerid), ref takerid);
                s.Get(nameof(taken), ref taken);
                s.Get(nameof(senderid), ref senderid);
                s.Get(nameof(sent), ref sent);
                s.Get(nameof(receiverid), ref receiverid);
                s.Get(nameof(received), ref received);
                s.Get(nameof(ended), ref ended);
            }
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(hubid), hubid);
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
            s.Put(nameof(paid), paid);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(shopid), shopid);
                s.Put(nameof(accepterid), accepterid);
                s.Put(nameof(accepted), accepted);
                s.Put(nameof(takerid), takerid);
                s.Put(nameof(taken), taken);
                s.Put(nameof(senderid), senderid);
                s.Put(nameof(sent), sent);
                s.Put(nameof(receiverid), receiverid);
                s.Put(nameof(received), received);
                s.Put(nameof(ended), ended);
            }
            s.Put(nameof(status), status);
        }
    }
}