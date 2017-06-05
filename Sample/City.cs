using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public class City : IData
    {
        internal string code;

        internal string name;

        internal string[] distrs;

        public void ReadData(IDataInput i, ushort proj = 0x00ff)
        {
            i.Get(nameof(code), ref code);
            i.Get(nameof(name), ref name);
            i.Get(nameof(distrs), ref distrs);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(code), code);
            o.Put(nameof(name), name);
            o.Put(nameof(distrs), distrs);
        }
    }
}