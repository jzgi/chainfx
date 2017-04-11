using Greatbone.Core;
using System;
using System.Collections.Generic;

namespace Greatbone.Sample
{
    /// 
    /// An order data object.
    ///
    public class Order : IData
    {
        // state
        public const int
            CREATED = 0,
            PAID = 1,
            LOCKED = 2,
            ABORTED = 4,
            CLOSED = 8,

            REASONED = 0x0100; // asked for cancelling

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [CREATED] = "新创建，等待付款",
            [PAID] = "已付款，等待处理",
            [LOCKED] = "已锁定",
            [ABORTED] = "已取消",
            [CLOSED] = "已结束",
        };


        public static readonly Order Empty = new Order();

        internal int id;
        internal string shopid;
        internal string shop; // shop name
        internal string shopwx; // shop openid
        internal string shoptel; // shop openid
        internal string buy; // buyer name or nickname
        internal string buywx; // buyer openid
        internal string buytel; // buyer telephone
        internal string buyaddr; // buyer shipping address
        internal List<OrderLine> lines;
        internal decimal total;
        internal short status;
        internal DateTime created; // time created
        internal DateTime paid; // time paid
        internal DateTime aborted; // time cancelled
        internal DateTime @fixed; // time start to handle
        internal DateTime closed; // time received or closed

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.AutoPrime())
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shop), ref shop);
            i.Get(nameof(shopwx), ref shopwx);
            i.Get(nameof(shoptel), ref shoptel);
            i.Get(nameof(buywx), ref buywx);
            i.Get(nameof(buy), ref buy);
            i.Get(nameof(buytel), ref buytel);
            i.Get(nameof(buyaddr), ref buyaddr);
            i.Get(nameof(total), ref total);
            if (proj.Detail())
            {
                i.Get(nameof(lines), ref lines);
            }
            i.Get(nameof(status), ref status);
            if (proj.Auto())
            {
                i.Get(nameof(created), ref created);
            }
            if (proj.Late())
            {
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(aborted), ref aborted);
                i.Get(nameof(@fixed), ref @fixed);
                i.Get(nameof(closed), ref closed);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Prime() && proj.Auto())
            {
                o.Put(nameof(id), id);
            }
            o.Put(nameof(shopid), shopid, label: "商家编号");
            o.Put(nameof(shop), shop, label: "商家名称");
            o.Put(nameof(shopwx), shopwx);
            o.Put(nameof(shoptel), shoptel, label: "  电话");
            o.Put(nameof(buywx), buywx);
            o.Put(nameof(buy), buy, label: "买家名称");
            o.Put(nameof(buytel), buytel, label: "  电话");
            o.Put(nameof(buyaddr), buyaddr, label: "  地址");
            o.Put(nameof(total), total, label: "金额");
            o.Put(nameof(created), created);
            if (proj.Detail())
            {
                o.Put(nameof(lines), lines);
            }
            o.Put(nameof(status), status, opt: STATUS);
            if (proj.Auto())
            {
                o.Put(nameof(created), created);
            }
            if (proj.Late())
            {
                o.Put(nameof(paid), paid);
                o.Put(nameof(aborted), aborted);
                o.Put(nameof(@fixed), @fixed);
                o.Put(nameof(closed), closed);
            }
        }

        public void AddItem(string item, short qty, decimal price, string note)
        {
            if (lines == null)
            {
                lines = new List<OrderLine>();
            }
            var orderln = lines.Find(o => o.item.Equals(item));
            if (orderln.item == null)
            {
                orderln = new OrderLine();
                lines.Add(orderln);
            }
        }
    }

    public struct OrderLine : IData
    {
        internal string item;
        internal short qty;
        internal decimal price;
        string note;

        public decimal Subtotal => price * qty;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(item), ref item);
            i.Get(nameof(qty), ref qty);
            i.Get(nameof(price), ref price);
            i.Get(nameof(note), ref note);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(item), item, label: "品名");
            o.Put(nameof(qty), qty, label: "数量");
            o.Put(nameof(price), price, label: "单价");
            o.Put(nameof(note), note, label: "附注");
        }

        public void AddQty(short qty)
        {
            this.qty += qty;
        }
    }

}