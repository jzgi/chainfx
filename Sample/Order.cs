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
        static readonly Dictionary<short, string> STATUS = new Dictionary<short, string>
        {
            [0] = null,
            [1] = "已付款",
            [2] = "已锁定",
            [3] = "已结束",
            [7] = "已取消",
        };


        public static readonly Order Empty = new Order();

        internal int id;

        internal string shopid;

        internal string shop; // shop name

        internal string shopwx; // shop weixin openid

        internal string userwx; // buyer weixin openid

        internal string user; // buyer nickname or name

        internal DateTime created; // time created

        internal string pend; // reason

        internal DateTime @fixed; // time fixed

        internal DateTime closed; // time closed

        OrderLine[] detail;

        decimal total;

        internal string payid; // payment id

        internal bool delivery;

        internal DateTime delivered;

        internal int state;
        internal short status;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(id), ref id);

            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shop), ref shop);
            i.Get(nameof(shopwx), ref shopwx);

            i.Get(nameof(user), ref user);
            i.Get(nameof(userwx), ref userwx);

            i.Get(nameof(created), ref created);
            if (proj.Sub())
            {
                i.Get(nameof(detail), ref detail);
            }
            i.Get(nameof(total), ref total);

            i.Get(nameof(delivery), ref delivery);
            i.Get(nameof(delivered), ref delivered);
            i.Get(nameof(state), ref state);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);

            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shop), shop);
            o.Put(nameof(shopwx), shopwx);

            o.Put(nameof(user), user);
            o.Put(nameof(userwx), userwx);

            o.Put(nameof(created), created);
            if (proj.Sub())
            {
                o.Put(nameof(detail), detail);
            }
            o.Put(nameof(total), total);

            o.Put(nameof(delivery), delivery, Options: b => b ? "派送" : "自提");
            o.Put(nameof(delivered), delivered);
            o.Put(nameof(state), state);
            o.Put(nameof(status), status, Options: STATUS);
        }

        public int State => state;
    }
}