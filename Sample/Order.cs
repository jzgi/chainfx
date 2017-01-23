using System;
using Greatbone.Core;
using static Greatbone.Core.FlagsUtility;

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

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(id), ref id);

            src.Get(nameof(shopid), ref shopid);
            src.Get(nameof(shop), ref shop);
            src.Get(nameof(shopwx), ref shopwx);

            src.Get(nameof(buyerid), ref buyerid);
            src.Get(nameof(buyer), ref buyer);
            src.Get(nameof(buyerwx), ref buyerwx);

            src.Get(nameof(opened), ref opened);
            if (flags.Has(SUB))
            {
                src.Get(nameof(lines), ref lines);
            }
            src.Get(nameof(total), ref total);

            src.Get(nameof(delivered), ref delivered);
            src.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(id), id);

            snk.Put(nameof(shopid), shopid);
            snk.Put(nameof(shop), shop);
            snk.Put(nameof(shopwx), shopwx);

            snk.Put(nameof(buyerid), buyerid);
            snk.Put(nameof(buyer), buyer);
            snk.Put(nameof(buyerwx), buyerwx);

            snk.Put(nameof(opened), opened);
            if (flags.Has(SUB))
            {
                snk.Put(nameof(lines), lines);
            }
            snk.Put(nameof(total), total);

            snk.Put(nameof(delivered), delivered);
            snk.Put(nameof(status), status);
        }
    }
}