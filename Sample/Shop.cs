using Greatbone.Core;
using static Greatbone.Core.FlagsUtility;

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

        public void Load(ISource src, byte flags = 0)
        {
            src.Get(nameof(id), ref id);
            src.Get(nameof(name), ref name);
            if (flags.Has(KEPT))
            {
                src.Get(nameof(credential), ref credential);
            }
            src.Get(nameof(tel), ref tel);
            src.Get(nameof(x), ref x);
            src.Get(nameof(y), ref y);
            src.Get(nameof(prov), ref prov);
            src.Get(nameof(city), ref city);
            src.Get(nameof(wx), ref wx);
            src.Get(nameof(notice), ref notice);
            src.Get(nameof(status), ref status);
        }

        public void Dump<R>(ISink<R> snk, byte flags = 0) where R : ISink<R>
        {
            snk.Put(nameof(id), id);
            snk.Put(nameof(name), name);
            if (flags.Has(KEPT))
            {
                snk.Put(nameof(credential), credential);
            }
            snk.Put(nameof(tel), tel);
            snk.Put(nameof(x), x);
            snk.Put(nameof(y), y);
            snk.Put(nameof(prov), prov);
            snk.Put(nameof(city), city);
            snk.Put(nameof(wx), wx);
            snk.Put(nameof(notice), notice);
            snk.Put(nameof(status), status);
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