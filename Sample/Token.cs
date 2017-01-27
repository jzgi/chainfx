using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An administrator's login.
    ///
    public class Token : IToken
    {
        // roles
        const short marketing = 1, accounting = 2, customer_service = 4;

        internal string key;

        internal string name;

        internal short subtype; // worker, shop, buyer

        internal short roles;

        public string Key => key;

        public string Name => name;

        public void Load(ISource s, byte flags = 0)
        {
            s.Get(nameof(key), ref key);
            s.Get(nameof(name), ref name);
            s.Get(nameof(subtype), ref subtype);
            s.Get(nameof(roles), ref roles);
        }

        public void Dump<R>(ISink<R> s, byte flags = 0) where R : ISink<R>
        {
            s.Put(nameof(key), key);
            s.Put(nameof(name), name);
            s.Put(nameof(subtype), subtype);
            s.Put(nameof(roles), roles);
        }
    }
}