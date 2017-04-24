using Greatbone.Core;

namespace Greatbone.Sample
{
    /// 
    ///
    public class City : IData
    {
        public static readonly Item Empty = new Item();

        internal string name;
        internal string code;
        internal string[] distrs;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(code), ref code);
            i.Get(nameof(distrs), ref distrs);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(code), code);
            o.Put(nameof(distrs), distrs);
        }
    }
}