using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An authorization token.
    ///
    public class Token : IData
    {
        // roles
        const short
            Marketing = 0x11,
            Accounting = 0x12,
            CustomerService = 0x14,
            Shop = 0x20,
            User = 0x40;

        internal string key;

        internal string name;

        internal short role;

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