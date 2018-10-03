﻿using Greatbone;

namespace Samp
{
    /// <summary>
    /// An item data object that represents a product or service.
    /// </summary>
    public class Item : IData
    {
        public static readonly Item Empty = new Item();

        public const byte PK = 1, LATER = 2;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "下架"},
            {1, "上架"},
            {2, "推荐"},
            {3, "置顶"},
        };

        internal short id;
        internal string hubid;
        internal string name;
        internal string descr;
        internal string remark;
        internal string mov; // movie url
        internal string unit;
        internal decimal price;
        internal decimal fee; // delivery fee
        internal decimal provp; // provision portion 
        internal decimal shipp; // shipping portion
        internal decimal teamp; // teaming portion
        internal short min;
        internal short step;
        internal bool refrig;
        internal int cap; // total capacity in 7 days
        internal int piled; // ordered but not fulfilled
        internal int provid;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(hubid), ref hubid);
            s.Get(nameof(name), ref name);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(remark), ref remark);
            s.Get(nameof(mov), ref mov);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(fee), ref fee);
            s.Get(nameof(provp), ref provp);
            s.Get(nameof(shipp), ref shipp);
            s.Get(nameof(teamp), ref teamp);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(refrig), ref refrig);
            if ((proj & LATER) > 0)
            {
                s.Get(nameof(piled), ref piled);
                s.Get(nameof(provid), ref provid);
                s.Get(nameof(cap), ref cap);
            }
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & PK) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(hubid), hubid);
            s.Put(nameof(name), name);
            s.Put(nameof(descr), descr);
            s.Put(nameof(remark), remark);
            s.Put(nameof(mov), mov);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(fee), fee);
            s.Put(nameof(provp), provp);
            s.Put(nameof(shipp), shipp);
            s.Put(nameof(teamp), teamp);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(refrig), refrig);
            if ((proj & LATER) > 0)
            {
                s.Put(nameof(piled), piled);
                s.Put(nameof(provid), provid);
                s.Put(nameof(cap), cap);
            }
            s.Put(nameof(status), status);
        }
    }
}