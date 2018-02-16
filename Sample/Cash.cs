using System;
using Greatbone.Core;

namespace Greatbone.Sample
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

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) == ID)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(shopid), ref shopid);
            s.Get(nameof(date), ref date);
            s.Get(nameof(txn), ref txn);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(received), ref received);
            s.Get(nameof(paid), ref paid);
            s.Get(nameof(keeper), ref keeper);
        }

        public void Write<R>(ISink<R> s, byte proj = 0x0f) where R : ISink<R>
        {
            if ((proj & ID) == ID)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(shopid), shopid);
            s.Put(nameof(date), date);
            s.Put(nameof(txn), txn);
            s.Put(nameof(descr), descr);
            s.Put(nameof(received), received);
            s.Put(nameof(paid), paid);
            s.Put(nameof(keeper), keeper);
        }
    }
}