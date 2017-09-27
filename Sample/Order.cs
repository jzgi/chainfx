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

        public const int
            ID = 0x0001,
            BASIC = 0x0002,
            BASIC_WX = 0x006,
            BASIC_DETAIL = 0x000A,
            CASH = 0x0100,
            FLOW = 0x0200;

        // status
        public const short CREATED = 0, ACCEPTED = 1, ABORTED = 3, SHIPPED = 5, RECKONED = 7;

        // status
        static readonly Map<short, string> STATUS = new Map<short, string>
        {
            [CREATED] = "购物车",
            [ACCEPTED] = "已付款",
            [ABORTED] = "已撤单",
            [SHIPPED] = "买家已确收/未清算",
            [RECKONED] = "平台已清算/完成",
        };


        internal long id;
        internal string shopid;
        internal string shopname;
        internal string wx; // weixin openid
        internal string name; // customer name
        internal string tel;
        internal string city;
        internal string addr; // address
        internal OrderItem[] items;
        internal decimal total; // receivable
        internal DateTime created; // time created

        internal decimal cash; // amount recieved
        internal DateTime accepted; // when cash received or forcibly accepted
        internal string abortly; // reason
        internal DateTime aborted; // time aborted
        internal DateTime shipped; // time shipped
        internal string note;
        internal short status;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            if ((proj & BASIC) == BASIC)
            {
                i.Get(nameof(shopid), ref shopid);
                i.Get(nameof(shopname), ref shopname);
                i.Get(nameof(name), ref name);
                if ((proj & BASIC_WX) == BASIC_WX)
                {
                    i.Get(nameof(wx), ref wx);
                }
                i.Get(nameof(city), ref city);
                i.Get(nameof(addr), ref addr);
                i.Get(nameof(tel), ref tel);
                if ((proj & BASIC_DETAIL) == BASIC_DETAIL)
                {
                    i.Get(nameof(items), ref items);
                }
                i.Get(nameof(total), ref total);
                i.Get(nameof(created), ref created);
            }
            if ((proj & CASH) == CASH)
            {
                i.Get(nameof(cash), ref cash);
            }
            if ((proj & FLOW) == FLOW)
            {
                i.Get(nameof(accepted), ref accepted);
                i.Get(nameof(abortly), ref abortly);
                i.Get(nameof(aborted), ref aborted);
                i.Get(nameof(shipped), ref shipped);
                i.Get(nameof(note), ref note);
            }
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id);
            }
            if ((proj & BASIC) == BASIC)
            {
                o.Put(nameof(created), created);
                o.Put(nameof(shopname), shopname);
                o.Put(nameof(shopid), shopid);
                o.Put(nameof(name), name);
                if ((proj & BASIC_WX) == BASIC_WX)
                {
                    o.Put(nameof(wx), wx);
                }
                o.Put(nameof(city), city);
                o.Put(nameof(addr), addr);
                o.Put(nameof(tel), tel);
                if ((proj & BASIC_DETAIL) == BASIC_DETAIL)
                {
                    o.Put(nameof(items), items);
                }
                o.Put(nameof(note), note);
                o.Put(nameof(total), total);
            }
            if ((proj & CASH) == CASH)
            {
                o.Put(nameof(cash), cash);
            }
            if ((proj & FLOW) == FLOW)
            {
                o.Put(nameof(accepted), accepted);
                o.Put(nameof(abortly), abortly);
                o.Put(nameof(aborted), aborted);
                o.Put(nameof(shipped), shipped);
            }
            o.Put(nameof(status), status);
        }

        public void AddItem(string item, short qty, string unit, decimal price)
        {
            if (items == null)
            {
                items = new[] {new OrderItem() {name = item, qty = qty, unit = unit, price = price}};
            }
            var orderln = items.Find(o => o.name.Equals(item));
            if (orderln != null)
            {
                orderln.qty += qty;
            }
            else
            {
                items = items.AddOf(new OrderItem() {name = item, qty = qty, unit = unit, price = price});
            }
        }

        public void Sum()
        {
            if (items != null)
            {
                decimal sum = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    sum += items[i].qty * items[i].price;
                }
                total = sum;
            }
        }

        public void SetLineQty(string name, short qty)
        {
            var ln = items.Find(x => x.name == name);
            if (ln != null)
            {
                ln.qty = qty;
            }
        }

        public void RemoveDetail(string name)
        {
            items = items.RemovedOf(x => x.name == name);
        }
    }

    public class OrderItem : IData
    {
        internal string name;
        internal short qty;
        internal string unit;
        internal decimal price;

        public decimal Subtotal => price * qty;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(unit), ref unit);
            i.Get(nameof(price), ref price);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(qty), qty);
            o.Put(nameof(unit), unit);
            o.Put(nameof(price), price);
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }
}