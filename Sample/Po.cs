using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A purchase order data object.
    /// </summary>
    public class Po : IData
    {
        public static readonly Po Empty = new Po();

        public const byte ID = 1;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "新建"},
            {1, "发布"},
        };

        internal int id;
        internal string ctrid;
        internal string vdrid;

        internal string item;
        internal string unit;
        internal decimal price;
        internal short qty;

        internal DateTime created;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(ctrid), ref ctrid);
            s.Get(nameof(vdrid), ref vdrid);

            s.Get(nameof(item), ref item);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(qty), ref qty);

            s.Get(nameof(created), ref created);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(ctrid), ctrid);
            s.Put(nameof(vdrid), vdrid);

            s.Put(nameof(item), item);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(qty), qty);

            s.Put(nameof(created), created);
            s.Put(nameof(status), status);
        }
    }
}