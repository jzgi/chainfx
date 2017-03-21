using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// An order data object.
    ///
    public class Order : IData, IStatable
    {
        // state
        public const int
            CREATED = 0,
            PAID = 1,
            LOCKED = 2,
            CANCELLED = 4,
            CLOSED = 8,

            REASONED = 0x0100; // asked for cancelling

        // status
        static readonly Map<short> STATUS = new Map<short>
        {
            [CREATED] = "新创建，等待付款",
            [PAID] = "已付款，等待处理",
            [LOCKED] = "处理中",
            [CANCELLED] = "已取消",
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
        List<OrderLine> lines;
        internal decimal total;
        internal short status;
        internal DateTime created; // time created
        internal DateTime paid; // time paid
        internal string reason; // for late cancelling
        internal DateTime cancelled; // time cancelled
        internal DateTime locked; // time start to handle
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
                i.Get(nameof(cancelled), ref cancelled);
                i.Get(nameof(locked), ref locked);
                i.Get(nameof(closed), ref closed);
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
            o.Put(nameof(status), status, Opt: STATUS);
            if (proj.Auto())
            {
                o.Put(nameof(created), created);
            }

            if (proj.Late())
            {
                o.Put(nameof(paid), paid);
                o.Put(nameof(cancelled), cancelled);
                o.Put(nameof(locked), locked);
                o.Put(nameof(closed), closed);
            }
        }

        public int State
        {
            get
            {
                int v = status;
                if (reason != null)
                {
                    v |= REASONED;
                }
                return v;
            }
        }

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