using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// A workflow that repays collected money to shops.
    ///
    public class Repay : IData
    {
        public const short

            // non-data or for control
            CTRL = 0x4000,

            // primary or key
            PRIME = 0x0800,

            // auto generated or with default
            AUTO = 0x0400,

            // binary
            BIN = 0x0200,

            // late-handled
            LATE = 0x0100,

            // many
            DETAIL = 0x0080,

            // transform or digest
            TRANSF = 0x0040,

            // secret or protected
            SECRET = 0x0020,

            // need authority
            POWER = 0x0010,

            // frozen or immutable
            IMMUT = 0x0008;


        // state
        public const int
            Prepared = 0,
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


        public static readonly Repay Empty = new Repay();

        internal int id; // platform shop id
        internal string shopid;
        internal DateTime time;
        internal decimal amount;
        internal decimal paid;
        internal string city;
        internal string wx;

        internal string endorderid;
        internal short status; // -1 dismissed, 0 closed, 1 open

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(time), ref time);
            i.Get(nameof(amount), ref amount);
            i.Get(nameof(paid), ref paid);
            i.Get(nameof(city), ref city);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(endorderid), ref endorderid);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(time), time);
            o.Put(nameof(amount), amount);
            o.Put(nameof(paid), paid);
            o.Put(nameof(city), city);
            o.Put(nameof(wx), wx);
            o.Put(nameof(endorderid), endorderid);
            o.Put(nameof(status), status);
        }
    }
}