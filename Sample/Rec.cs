using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// An journal record of cash receipt or payment.
    /// </summary>
    public class Rec : IData
    {
        ///
        public static readonly Rec Empty = new Rec();

        ///
        public const byte ID = 1;

        ///
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
        internal string orgid;
        internal DateTime date;
        internal short code;
        internal string descr;
        internal decimal receive;
        internal decimal pay;
        internal string creator;

        ///
        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) == ID)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(orgid), ref orgid);
            s.Get(nameof(date), ref date);
            s.Get(nameof(code), ref code);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(receive), ref receive);
            s.Get(nameof(pay), ref pay);
            s.Get(nameof(creator), ref creator);
        }

        ///
        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) == ID)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(orgid), orgid);
            s.Put(nameof(date), date);
            s.Put(nameof(code), code);
            s.Put(nameof(descr), descr);
            s.Put(nameof(receive), receive);
            s.Put(nameof(pay), pay);
            s.Put(nameof(creator), creator);
        }
    }
}