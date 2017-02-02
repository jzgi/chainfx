using System;
using Greatbone.Core;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    /// 
    /// An order data object.
    ///
    public class Order : IData
    {
        public const short
            Open = 0,
            Cancelled = 2,
            Closed = 9;

        public static readonly Order Empty = new Order();

        internal int id;

        internal string shopid;

        internal string shop; // shop name

        internal string shopwx; // shop weixin openid

        internal string buyerid; // RESERVED

        internal string buyer; // buyer nickname or name

        internal string buyerwx; // buyer weixin openid

        internal DateTime opened;

        OrderLine[] lines;

        decimal total;

        internal string payid; // payment id

        internal DateTime delivered;

        internal int status;

        public void ReadData(IDataInput i, ushort proj = 0)
        {
            i.Get(nameof(id), ref id);

            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shop), ref shop);
            i.Get(nameof(shopwx), ref shopwx);

            i.Get(nameof(buyerid), ref buyerid);
            i.Get(nameof(buyer), ref buyer);
            i.Get(nameof(buyerwx), ref buyerwx);

            i.Get(nameof(opened), ref opened);
            if (proj.Sub())
            {
                i.Get(nameof(lines), ref lines);
            }
            i.Get(nameof(total), ref total);

            i.Get(nameof(delivered), ref delivered);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);

            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shop), shop);
            o.Put(nameof(shopwx), shopwx);

            o.Put(nameof(buyerid), buyerid);
            o.Put(nameof(buyer), buyer);
            o.Put(nameof(buyerwx), buyerwx);

            o.Put(nameof(opened), opened);
            if (proj.Sub())
            {
                o.Put(nameof(lines), lines);
            }
            o.Put(nameof(total), total);

            o.Put(nameof(delivered), delivered);
            o.Put(nameof(status), status);
        }
    }
}