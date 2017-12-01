using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// An journal entry of cash receipt or payment.
    /// </summary>
    public class Cash : IData
    {
        public const short ID = 1;
        
        public static readonly Cash Empty = new Cash();

        public static readonly Map<short, string> Codes = new Map<short, string>
        {
            {11, "销售收入"},
            {19, "其他收入"},
            {21, "材料支出"},
            {22, "耗品支出"},
            {23, "工资支出"},
            {24, "设备支出"},
            {29, "其他支出"},
        };

        internal int id;
        internal string shopid;
        internal DateTime date;
        internal short code;
        internal string descr;
        internal decimal received;
        internal decimal paid;
        internal string recorder;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            if ((proj & ID) == ID)
            {
                i.Get(nameof(id), ref id);
            }
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(date), ref date);
            i.Get(nameof(code), ref code);
            i.Get(nameof(descr), ref descr);
            i.Get(nameof(received), ref received);
            i.Get(nameof(paid), ref paid);
            i.Get(nameof(recorder), ref recorder);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            if ((proj & ID) == ID)
            {
                o.Put(nameof(id), id);
            }
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(date), date);
            o.Put(nameof(code), code);
            o.Put(nameof(descr), descr);
            o.Put(nameof(received), received);
            o.Put(nameof(paid), paid);
            o.Put(nameof(recorder), recorder);
        }
    }
}