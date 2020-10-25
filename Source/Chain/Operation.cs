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

        internal string tn;
        internal short step;
        
        internal string an; // account number
        internal short typ;
        internal string inst;
        
        internal string descr;
        internal decimal amt;
        internal JObj doc;
        internal DateTime stamp;
        internal short status;

        internal string npeerid;
        internal string nan;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(tn), ref tn);
            s.Get(nameof(step), ref step);
            s.Get(nameof(an), ref an);
            s.Get(nameof(typ), ref typ);
            s.Get(nameof(inst), ref inst);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(status), ref status);
            s.Get(nameof(npeerid), ref npeerid);
            s.Get(nameof(nan), ref nan);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(tn), tn);
            s.Put(nameof(step), step);
            s.Put(nameof(an), an);
            s.Put(nameof(typ), typ);
            s.Put(nameof(inst), inst);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(doc), doc);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(status), status);
            s.Put(nameof(npeerid), npeerid);
            s.Put(nameof(nan), nan);
        }
    }
}