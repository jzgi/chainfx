using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An administrator's login.
    ///
    public class Token : IData
    {
        // roles
        const short marketing = 1, accounting = 2, customer_service = 4;

        internal string key;

        internal string name;

        internal short subtype; // worker, shop, buyer

        internal short roles;

        public string Key => key;

        public string Name => name;

        public void ReadData(IDataInput i, byte flags = 0)
        {
            i.Get(nameof(key), ref key);
            i.Get(nameof(name), ref name);
            i.Get(nameof(subtype), ref subtype);
            i.Get(nameof(roles), ref roles);
        }

        public void WriteData<R>(IDataOutput<R> o, byte flags = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(key), key);
            o.Put(nameof(name), name);
            o.Put(nameof(subtype), subtype);
            o.Put(nameof(roles), roles);
        }
    }
}