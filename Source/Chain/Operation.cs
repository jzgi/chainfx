using System;

namespace SkyChain.Chain
{
    public class Operation : IData
    {
        public static readonly Operation Empty = new Operation();

        public const byte ID = 1, PRIVACY = 2;


        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {-1, "否决"},
            {0, null},
            {1, "进行"},
            {2, "通过"},
            {3, "存档"},
        };

        internal int id;
        internal string acct;
        internal string npeerid;
        internal string nacct;
        internal short typ;
        internal int inst;
        internal short step;
        internal decimal descr;
        internal decimal amt;
        internal JObj doc;
        internal DateTime created;
        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(npeerid), ref npeerid);
            s.Get(nameof(nacct), ref nacct);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(inst), ref inst);
            s.Get(nameof(step), ref step);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(created), ref created);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(acct), acct);
            s.Put(nameof(npeerid), npeerid);
            s.Put(nameof(nacct), nacct);
            s.Put(nameof(typ), typ);
            s.Put(nameof(inst), inst);
            s.Put(nameof(step), step);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(doc), doc);
            s.Put(nameof(created), created);
            s.Put(nameof(status), status);
        }
    }
}