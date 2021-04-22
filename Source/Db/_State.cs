using System;

namespace SkyChain.Db
{
    /// <summary>
    /// A workflow state or step data record.
    /// </summary>
    public class _State : IData
    {
        public static readonly _State Empty = new _State();

        internal long job;
        internal short step;
        internal string acct;
        internal string name;
        internal string ldgr;
        internal string descr;
        internal decimal amt;
        internal JObj doc;
        internal DateTime stated;
        internal short ppid;
        internal string pacct;
        internal string pname;
        internal short npid;
        internal string nacct;
        internal string nname;
        internal DateTime stamp;

        public virtual void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(job), ref job);
            s.Get(nameof(step), ref step);
            s.Get(nameof(acct), ref acct);
            s.Get(nameof(name), ref name);
            s.Get(nameof(ldgr), ref ldgr);
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(amt), ref amt);
            s.Get(nameof(doc), ref doc);
            s.Get(nameof(stated), ref stated);
            s.Get(nameof(ppid), ref ppid);
            s.Get(nameof(pacct), ref pacct);
            s.Get(nameof(pname), ref pname);
            s.Get(nameof(npid), ref npid);
            s.Get(nameof(nacct), ref nacct);
            s.Get(nameof(nname), ref nname);
            s.Get(nameof(stamp), ref stamp);
        }

        public virtual void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(job), job);
            s.Put(nameof(step), step);
            s.Put(nameof(acct), acct);
            s.Put(nameof(name), name);
            s.Put(nameof(ldgr), ldgr);
            s.Put(nameof(descr), descr);
            s.Put(nameof(amt), amt);
            s.Put(nameof(doc), doc);
            s.Put(nameof(stated), stated);
            if (ppid == 0) s.PutNull(nameof(ppid));
            else s.Put(nameof(ppid), ppid);
            s.Put(nameof(pacct), pacct);
            s.Put(nameof(pname), pname);
            if (npid == 0) s.PutNull(nameof(npid));
            else s.Put(nameof(npid), npid);
            s.Put(nameof(nacct), nacct);
            s.Put(nameof(nname), nname);
            s.Put(nameof(stamp), stamp);
        }

        public long Job => job;

        public short Step => step;

        public string Acct => acct;

        public string Name => name;

        public string Ldgr => ldgr;

        public string Descr => descr;

        public decimal Amt => amt;

        public JObj Doc => doc;

        public DateTime Stated => stated;

        public bool IsLocal => ppid == 0;

        public short PrevPeerId => ppid;

        public string PrevAcct => pacct;

        public string PrevName => pname;

        public short NextPeerId => npid;

        public string NextAcct => nacct;

        public string NextName => nname;
    }
}