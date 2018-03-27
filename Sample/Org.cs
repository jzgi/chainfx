using System.Collections.Generic;
using Greatbone;

namespace Core
{
    public class Org : IData, IKeyable<string>
    {
        public static readonly Org Empty = new Org();

        public const byte ADM = 3, ID = 1, NORM = 4, LATER = 8;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "停业状态"},
            {1, "暂缓接单"},
            {2, "正常接单"}
        };

        internal string id;
        internal string name;
        internal string descr;
        internal string addr;
        internal double x;
        internal double y;
        internal string mgrwx;
        internal string mgrtel;
        internal string mgrname;
        internal string oprwx;
        internal string oprtel;
        internal string oprname;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) s.Get(nameof(id), ref id);
                s.Get(nameof(name), ref name);
                s.Get(nameof(descr), ref descr);
                s.Get(nameof(addr), ref addr);
                s.Get(nameof(x), ref x);
                s.Get(nameof(y), ref y);
            }
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(mgrwx), ref mgrwx);
                s.Get(nameof(mgrtel), ref mgrtel);
                s.Get(nameof(mgrname), ref mgrname);
                s.Get(nameof(oprwx), ref oprwx);
                s.Get(nameof(oprtel), ref oprtel);
                s.Get(nameof(oprname), ref oprname);
                s.Get(nameof(status), ref status);
            }
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ADM) != 0)
            {
                if ((proj & ID) == ID) s.Put(nameof(id), id);
                s.Put(nameof(name), name);
                s.Put(nameof(descr), descr);
                s.Put(nameof(addr), addr);
                s.Put(nameof(x), x);
                s.Put(nameof(y), y);
            }
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(mgrwx), mgrwx);
                s.Put(nameof(mgrtel), mgrtel);
                s.Put(nameof(mgrname), mgrname);
                s.Put(nameof(oprwx), oprwx);
                s.Put(nameof(oprtel), oprtel);
                s.Put(nameof(oprname), oprname);
                s.Put(nameof(status), status);
            }
        }

        public string Key => id;
    }
}