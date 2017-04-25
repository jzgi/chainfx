using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

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

        public Opt<short> STATUS = new Opt<short>
        {
            [0] = "停业",
            [1] = "休假中",
            [2] = "营业中"
        };

        internal string id;
        internal string name;
        internal string credential;
        internal string descr;
        internal string tel;
        internal string city;
        internal string distr;
        internal string addr;
        internal double x;
        internal double y;
        internal string lic;
        internal DateTime created;
        internal int orders;
        internal short status;

        public void ReadData(IDataInput i, short proj = 0)
        {
            if ((proj & PRIME) == PRIME)
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(name), ref name);
            if ((proj & TRANSF) == TRANSF)
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(descr), ref descr);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(city), ref city);
            i.Get(nameof(distr), ref distr);
            i.Get(nameof(addr), ref addr);
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            if ((proj & IMMUT) == IMMUT)
            {
                i.Get(nameof(lic), ref lic);
            }
            i.Get(nameof(orders), ref orders);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
            if ((proj & PRIME) == PRIME)
            {
                o.Put(nameof(id), id, label: "编号", required: true);
            }
            o.Put(nameof(name), name, label: "名称");
            if ((proj & TRANSF) == TRANSF)
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(descr), descr, label: "简语");
            o.Put(nameof(tel), tel, label: "电话", max: 11);
            o.Put(nameof(city), city, label: "城市", max: 10);
            o.Put(nameof(distr), distr, label: "区县", max: 10);
            o.Put(nameof(addr), addr, label: "地址", max: 10);
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            if ((proj & IMMUT) == IMMUT)
            {
                o.Put(nameof(lic), lic, label: "工商登记");
            }
            o.Put(nameof(orders), orders);
            o.Put(nameof(status), status, label: "状态", opt: STATUS);
        }
    }
}