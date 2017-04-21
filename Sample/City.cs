using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    ///
    public class City : IData
    {
        public static readonly Item Empty = new Item();

        internal string name;
        internal string[] distrs;
        internal string code;

        public void ReadData(IDataInput i, int proj = 0)
        {
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
        }
    }
}