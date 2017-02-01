using Greatbone.Core;
using static Greatbone.Core.Flags;

namespace Greatbone.Sample
{
    ///
    /// A shop data object.
    ///
    public class Shop : IData
    {
        public static readonly Shop Empty = new Shop();

        internal string id; // platform shop id
        internal string name;
        internal string credential;
        internal string tel;
        internal double x, y;
        internal string prov;
        internal string city;
        internal string wx;
        internal string notice;
        internal short status; // -1 dismissed, 0 closed, 1 open

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void ReadData(IDataInput i, byte flags = 0)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(name), ref name);
            if (flags.Has(KEPT))
            {
                i.Get(nameof(credential), ref credential);
            }
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(x), ref x);
            i.Get(nameof(y), ref y);
            i.Get(nameof(prov), ref prov);
            i.Get(nameof(city), ref city);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(notice), ref notice);
            i.Get(nameof(status), ref status);
        }

        public void WriteData<R>(IDataOutput<R> o, byte flags = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(name), name);
            if (flags.Has(KEPT))
            {
                o.Put(nameof(credential), credential);
            }
            o.Put(nameof(tel), tel);
            o.Put(nameof(x), x);
            o.Put(nameof(y), y);
            o.Put(nameof(prov), prov);
            o.Put(nameof(city), city);
            o.Put(nameof(wx), wx);
            o.Put(nameof(notice), notice);
            o.Put(nameof(status), status);
        }

        public Token ToToken()
        {
            return new Token()
            {
                key = wx,
                name = name,
                subtype = 2
            };
        }
    }
}