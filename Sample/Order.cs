using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// An order data object.
    /// </summary>
    public class Order : IData, IKeyable<int>
    {
        public static readonly Order Empty = new Order();

        public const byte ID = 1, LATER = 4;

        // status
        public const short
            ABORTED = -1, CREATED = 0, PAID = 1, CONFIRMED = 2, ACCEPTED = 3, SENT = 4, RECEIVED = 5, ENDED = 6;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {ABORTED, "撤销"},
            {CREATED, null},
            {PAID, "排队"},
            {CONFIRMED, "备货"},
            {ACCEPTED, "中转"},
            {SENT, "派运"},
            {RECEIVED, "运达"},
            {ENDED, "完成"}
        };

        internal int id;
        internal string hubid;
        internal int custid;
        internal string cust; // customer name
        internal string custtel;
        internal string custaddr; // may include area and site
        internal short teamid;
        internal short itemid;
        internal string item;
        internal string unit;
        internal decimal price;
        internal short qty;
        internal decimal total; // total
        internal decimal cash; // cash = total paid
        internal int creatorid;
        internal string creator;
        internal string creatorwx; // weixin openid

        internal DateTime paid;
        internal short shopid; // workshop's orgid
        internal int confirmerid; // confirm the order and start preparation
        internal DateTime confirmed;
        internal int accepterid; // check and accpet goods from the provider 
        internal DateTime accepted;
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
            s.Get(nameof(custid), ref custid);
            s.Get(nameof(cust), ref cust);
            s.Get(nameof(custtel), ref custtel);
            s.Get(nameof(custaddr), ref custaddr);
            s.Get(nameof(teamid), ref teamid);
            s.Get(nameof(itemid), ref itemid);
            s.Get(nameof(item), ref item);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);
            s.Get(nameof(total), ref total);
            s.Get(nameof(cash), ref cash);
            s.Get(nameof(creatorid), ref creatorid);
            s.Get(nameof(creator), ref creator);
            s.Get(nameof(creatorwx), ref creatorwx);
            s.Get(nameof(paid), ref paid);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(shopid), ref shopid);
                s.Get(nameof(confirmerid), ref confirmerid);
                s.Get(nameof(confirmed), ref confirmed);
                s.Get(nameof(accepterid), ref accepterid);
                s.Get(nameof(accepted), ref accepted);
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
            s.Put(nameof(custid), custid);
            s.Put(nameof(cust), cust);
            s.Put(nameof(custtel), custtel);
            s.Put(nameof(custaddr), custaddr);
            s.Put(nameof(teamid), teamid);
            s.Put(nameof(itemid), itemid);
            s.Put(nameof(item), item);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);
            s.Put(nameof(total), total);
            s.Put(nameof(cash), cash);
            s.Put(nameof(creatorid), creatorid);
            s.Put(nameof(creator), creator);
            s.Put(nameof(creatorwx), creatorwx);
            s.Put(nameof(paid), paid);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(shopid), shopid);
                s.Put(nameof(confirmerid), confirmerid);
                s.Put(nameof(confirmed), confirmed);
                s.Put(nameof(accepterid), accepterid);
                s.Put(nameof(accepted), accepted);
                s.Put(nameof(senderid), senderid);
                s.Put(nameof(sent), sent);
                s.Put(nameof(receiverid), receiverid);
                s.Put(nameof(received), received);
                s.Put(nameof(ended), ended);
            }
            s.Put(nameof(status), status);
        }

        public int Key => id;
    }
}