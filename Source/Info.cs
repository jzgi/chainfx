using System;

namespace CoChain
{
    /// <summary>
    /// A data model that represents a general piece of information.
    /// </summary>
    public abstract class Info : IData
    {
        public const short
            STA_DEAD = -1,
            STA_DISABLED = 0,
            STA_ENABLED = 1,
            STA_HOT = 2;

        public static readonly Map<short, string> States = new Map<short, string>
        {
            {STA_DEAD, "消灭"},
            {STA_DISABLED, "停用"},
            {STA_ENABLED, "可用"},
            {STA_HOT, "热榜"},
        };

        public const short
            ID = 0x0001,
            TYP = 0x0002,
            BORN = 0x0004,
            DUAL = 0x0008,
            LATER = 0x0010,
            EXTRA = 0x0100;


        public short typ;
        public short state;
        public string name;
        public string tip;
        public DateTime created;
        public string creator;
        public DateTime adapted;
        public string adapter;

        public virtual void Read(ISource s, short msk = 0xff)
        {
            if ((msk & BORN) == BORN)
            {
                s.Get(nameof(typ), ref typ);
                s.Get(nameof(created), ref created);
                s.Get(nameof(creator), ref creator);
            }
            s.Get(nameof(state), ref state);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tip), ref tip);
            if ((msk & LATER) == LATER)
            {
                s.Get(nameof(adapted), ref adapted);
                s.Get(nameof(adapter), ref adapter);
            }
        }

        public virtual void Write(ISink s, short msk = 0xff)
        {
            if ((msk & BORN) == BORN)
            {
                s.Put(nameof(typ), typ);
                s.Put(nameof(created), created);
                s.Put(nameof(creator), creator);
            }
            s.Put(nameof(state), state);
            s.Put(nameof(name), name);
            s.Put(nameof(tip), tip);
            if ((msk & LATER) == LATER)
            {
                s.Put(nameof(adapted), adapted);
                s.Put(nameof(adapter), adapter);
            }
        }

        public virtual bool IsDead => state == STA_DEAD;

        public virtual bool IsDisabled => state == STA_DISABLED;

        public virtual bool IsShowable => state >= STA_DISABLED;

        public virtual bool IsEnabled => state == STA_ENABLED;

        public virtual bool IsWorkable => state >= STA_ENABLED;

        public virtual bool IsHot => state == STA_HOT;
    }
}