using System;

namespace SkyChain.Chain
{
    /// <summary>
    /// An operational record.
    /// </summary>
    public class Operational : IData, IDualKeyable<long, short>
    {
        public static readonly Operational Empty = new Operational();

        public const byte ID = 1, PRIVACY = 2;

        public const short
            STARTED = 0b000,
            ABORTED = 0b001,
            FORTH_IN = 0b010,
            FORTH_OUT = 0b011,
            BACK_IN = 0b100,
            BACK_OUT = 0b101,
            ENDED = 0b111;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STARTED, "开始"},
            {ABORTED, "撤销"},
            {FORTH_IN, "送入"},
            {FORTH_OUT, "送出"},
            {BACK_IN, "退来"},
            {BACK_OUT, "退走"},
            {ENDED, "结束"},
        };

        internal long job; // job + step is globally-unique op number
        internal short step;
        internal string acct;
        internal string name;
        internal string ldgr;
        internal string descr;
        internal decimal amt;
        internal decimal bal;
        internal JObj doc;
        internal DateTime stated;
        internal short ppeerid;
        internal string pacct;
        internal string pname;
        internal short npeerid;
        internal string nacct;
        internal string nname;
        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(job), ref job);
            s.Get(nameof(step), ref step);
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(name), ref name);
            s.Get(nameof(ldgr), ref ldgr);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(bal), ref bal);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(stated), ref stated);
            s.Get(nameof(ppeerid), ref ppeerid);
            s.Get(nameof(pacct), ref pacct);
            s.Get(nameof(pname), ref pname);
            s.Get(nameof(npeerid), ref npeerid);
            s.Get(nameof(nacct), ref nacct);
            s.Get(nameof(nname), ref nname);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(job), job);
            s.Put(nameof(step), step);
            s.Put(nameof(acct), acct);
            s.Put(nameof(name), name);
            s.Put(nameof(ldgr), ldgr);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(bal), bal);
            s.Put(nameof(doc), doc);
            s.Put(nameof(stated), stated);
            s.Put(nameof(ppeerid), ppeerid);
            s.Put(nameof(pacct), pacct);
            s.Put(nameof(pname), pname);
            s.Put(nameof(npeerid), npeerid);
            s.Put(nameof(nacct), nacct);
            s.Put(nameof(nname), nname);
            s.Put(nameof(status), status);
        }

        public long Job => job;

        public short Step => step;

        public string Acct => acct;

        public string Name => name;

        public string Ldgr => ldgr;

        public string Descr => descr;

        public decimal Amt => amt;

        public decimal Bal => bal;

        public JObj Doc => doc;

        public DateTime Stated => stated;

        public bool IsLocal => ppeerid == 0;

        public short PrevPeerId => ppeerid;

        public string PrevAcct => pacct;

        public string PrevName => pname;

        public short NextPeerId => npeerid;

        public string NextAcct => nacct;

        public string NextName => nname;

        public short Status => status;

        public bool IsPresent => (status & 0b001) == 0;

        public long Key => job;

        public (long, short) CompositeKey => (job, step);
    }
}