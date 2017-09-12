using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A city data object.
    /// </summary>
    public class City : IData
    {
        internal string code;
        internal string name;
        internal string[] distrs;

        public void Read(IDataInput i, int proj = 0x00ff)
        {
            i.Get(nameof(code), ref code);
            i.Get(nameof(name), ref name);
            i.Get(nameof(distrs), ref distrs);
        }

        public void Write<R>(IDataOutput<R> o, int proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(code), code);
            o.Put(nameof(name), name);
            o.Put(nameof(distrs), distrs);
        }

        public override string ToString()
        {
            return name;
        }
    }
}