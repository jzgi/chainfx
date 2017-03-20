using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// An order processing workflow.
    ///
    public class Order : IData, IStatable
    {
        // state
        public const int
            INITIAL = 0,
            PAID = 1,
            ASKED = 2,
            FIXED = 4,
            CLOSED = 4,
            CANCELLED = 8;

        // status
        static readonly Map<short> STATUS = new Map<short>
        {
            [0] = "初始",
            [1] = "已付款，等待处理",
            [2] = "处理中",
            [3] = "已结束",
            [7] = "已取消",
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
        internal decimal total;
        List<OrderLine> lines;
        internal DateTime created; // time created
        internal DateTime paid; // time paid
        internal DateTime cancelled; // time cancelled
        internal DateTime handled; // time start to handle
        internal DateTime closed; // time received or closed
        internal short state;
        internal short status;

        public void ReadData(IDataInput i, int proj = 0)
        {
            if (proj.Prime() && proj.Auto())
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
            if (proj.Auto())
            {
                i.Get(nameof(created), ref created);
            }
            if (proj.Late())
            {
                i.Get(nameof(paid), ref paid);
                i.Get(nameof(cancelled), ref cancelled);
                i.Get(nameof(handled), ref handled);
                i.Get(nameof(closed), ref closed);
            }
            if (proj.Stat())
            {
                i.Get(nameof(state), ref state);
                i.Get(nameof(status), ref status);
            }
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            if (proj.Prime() && proj.Auto())
            {
                o.Put(nameof(id), id);
            }
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shop), shop);
            o.Put(nameof(shopwx), shopwx);
            o.Put(nameof(shoptel), shoptel);
            o.Put(nameof(buywx), buywx);
            o.Put(nameof(buy), buy);
            o.Put(nameof(buytel), buytel);
            o.Put(nameof(buyaddr), buyaddr);
            o.Put(nameof(total), total);
            o.Put(nameof(created), created);
            if (proj.Detail())
            {
                o.Put(nameof(lines), lines);
            }
            if (proj.Auto())
            {
                o.Put(nameof(created), created);
            }

            if (proj.Late())
            {
                o.Put(nameof(paid), paid);
                o.Put(nameof(cancelled), cancelled);
                o.Put(nameof(handled), handled);
                o.Put(nameof(closed), closed);
            }
            if (proj.Stat())
            {
                o.Put(nameof(state), state);
                o.Put(nameof(status), status, Opt: STATUS);
            }
        }

        public int State => state;

        public void add(string item, short qty, decimal price, string note)
        {
            if (lines == null)
            {
                lines = new List<OrderLine>();
            }
            // var orderln = lines.Find(o => o.shopid.Equals(shopid));
            // if (orderln == null)
            // {
            //     orderln = new OrderLine();
            //     Add(order);
            // }
        }

    }
}