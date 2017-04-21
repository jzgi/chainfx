using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    /// A product or service.
    ///
    public class Complaint : IData
    {
        public static readonly Item Empty = new Item();

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [0] = "未处理",
            [1] = "已处理",
            [2] = "在售",
        };

        internal string shopid;
        internal string name;
        internal string descr;
        internal ArraySegment<byte> icon;
        internal string unit;
        internal decimal price; // current price
        internal int min; // minimal ordered
        internal int step;
        internal short status;

        public void ReadData(IDataInput i, int proj = 0)
        {
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
        }
    }
}