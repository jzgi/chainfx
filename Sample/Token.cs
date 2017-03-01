using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// An authorization token.
    ///
    public class Token : IData
    {
        // roles
        public const int

            // admins
            MKTG = 0x11,
            ACCTG = 0x12,
            CUSTSVC = 0x14,
            SHOP = 0x20,
            USER = 0x40;

        internal string key;

        internal string name;

        internal string wx;

        internal int roles;

        internal string extra;


        public string Key => key;

        public bool IsAdmin => (roles & 0x10) == 0x10;

        public bool IsShop => (roles & SHOP) == SHOP;

        public bool IsUser => (roles & USER) == USER;

        public void ReadData(IDataInput i, int proj = 0)
        {
            i.Get(nameof(key), ref key);
            i.Get(nameof(name), ref name);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(roles), ref roles);
            i.Get(nameof(extra), ref extra);
        }

        public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
        {
            o.Put(nameof(key), key);
            o.Put(nameof(name), name);
            o.Put(nameof(wx), wx);
            o.Put(nameof(roles), roles);
            o.Put(nameof(extra), extra);
        }
    }
}