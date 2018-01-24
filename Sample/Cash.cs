using System;
using Greatbone.Core;

namespace Greatbone.Samp
{
    /// <summary>
    /// An journal entry of cash receipt or payment.
    /// </summary>
    public class Cash : IData
    {
        public static readonly Cash Empty = new Cash();

        public const byte ID = 1;

        public static readonly Map<short, string> Codes = new Map<short, string>
        {
            {11, "销售收入"},
            {19, "其他收入"},
            {21, "场所支出"},
            {22, "设备支出"},
            {23, "材料支出"},
            {24, "耗品支出"},
            {25, "工资支出"},
            {29, "其他支出"},
        };

        internal int id;
        internal string shopid;
        internal DateTime date;
        internal short txn;
        internal string descr;
        internal decimal received;
        internal decimal paid;
        internal string keeper;

        public void Read(IDataInput i, byte proj = 0x1f)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(date), ref date);
            i.Get(nameof(txn), ref txn);
            i.Get(nameof(descr), ref descr);
            i.Get(nameof(received), ref received);
            i.Get(nameof(paid), ref paid);
            i.Get(nameof(keeper), ref keeper);
        }

        public void Write<R>(IDataOutput<R> o, byte proj = 0x1f) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id);
            }
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(date), date);
            o.Put(nameof(txn), txn);
            o.Put(nameof(descr), descr);
            o.Put(nameof(received), received);
            o.Put(nameof(paid), paid);
            o.Put(nameof(keeper), keeper);
        }
    }
}