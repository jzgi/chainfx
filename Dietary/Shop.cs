using Greatbone.Core;
using static Greatbone.Core.ZUtility;

namespace Ministry.Dietary
{
    ///
    /// A shop data object.
    ///
    public class Shop : IPrincipal, IData
    {
        public static readonly Shop Empty = new Shop();

        internal string id;
        internal string name;
        internal string credential;
        internal string tel;
        internal string addr;
        internal bool disabled;

        public string Key => id;

        public string Name => name;

        public string Credential => credential;

        public void Load(ISource s, byte z = 0)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            if (z.Ya(RESV)) s.Get(nameof(credential), ref credential);
            s.Get(nameof(tel), ref tel);
            s.Get(nameof(addr), ref addr);
            s.Get(nameof(disabled), ref disabled);
        }

        public void Dump<R>(ISink<R> s, byte z = 0) where R : ISink<R>
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            if (z.Ya(RESV)) s.Put(nameof(credential), credential);
            s.Put(nameof(tel), tel);
            s.Put(nameof(addr), addr);
            s.Put(nameof(disabled), disabled);
        }
    }
}