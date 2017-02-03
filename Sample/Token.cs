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

        internal short role; // worker, shop, user

        internal string extra;

        public void ReadData(IDataInput i, ushort proj = 0)
        {
            i.Get(nameof(key), ref key);
            i.Get(nameof(name), ref name);
            i.Get(nameof(role), ref role);
            i.Get(nameof(extra), ref extra);
        }

        public void WriteData<R>(IDataOutput<R> o, ushort proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(key), key);
            o.Put(nameof(name), name);
            o.Put(nameof(role), role);
            o.Put(nameof(extra), extra);
        }
    }
}