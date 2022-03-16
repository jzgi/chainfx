using System;

namespace SkyChain
{
    /// <summary>
    /// A data model for general unit of information.
    /// </summary>
    public abstract class Info : IData
    {
        public const short
            STA_GONE = -1,
            STA_DISABLED = 0,
            STA_ENABLED = 1,
            STA_PREFERRED = 2;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STA_GONE, "注销"},
            {STA_DISABLED, "禁用"},
            {STA_ENABLED, "正常"},
            {STA_PREFERRED, "优先"},
        };

        public static readonly Map<short, string> Symbols = new Map<short, string>
        {
            {STA_GONE, "注销"},
            {STA_DISABLED, "禁用"},
            {STA_ENABLED, null},
            {STA_PREFERRED, null},
        };

        public const short
            ID = 0x0001,
            TYP = 0x0002,
            BORN = 0x0004,
            DUAL = 0x0008,
            LATER = 0x0010,
            EXTRA = 0x0100;


        internal short typ;
        internal short status;
        internal string name;
        internal string tip;
        internal DateTime created;
        internal string creator;
        internal DateTime adapted;
        internal string adapter;

        public virtual void Read(ISource s, short proj = 0xff)
        {
            if ((proj & TYP) == TYP || (proj & BORN) == BORN)
            {
                s.Get(nameof(typ), ref typ);
            }
            if ((proj & BORN) == BORN)
            {
                s.Get(nameof(created), ref created);
                s.Get(nameof(creator), ref creator);
            }
            s.Get(nameof(status), ref status);
            s.Get(nameof(name), ref name);
            s.Get(nameof(tip), ref tip);
            if ((proj & LATER) == LATER)
            {
                s.Get(nameof(adapted), ref adapted);
                s.Get(nameof(adapter), ref adapter);
            }
        }

        public virtual void Write(ISink s, short proj = 0xff)
        {
            if ((proj & TYP) == TYP || (proj & BORN) == BORN)
            {
                s.Put(nameof(typ), typ);
            }
            if ((proj & BORN) == BORN)
            {
                s.Put(nameof(created), created);
                s.Put(nameof(creator), creator);
            }
            s.Put(nameof(status), status);
            s.Put(nameof(name), name);
            s.Put(nameof(tip), tip);
            if ((proj & LATER) == LATER)
            {
                s.Put(nameof(adapted), adapted);
                s.Put(nameof(adapter), adapter);
            }
        }

        public virtual bool IsGone => status <= STA_GONE;

        public virtual bool IsDisabled => status == STA_DISABLED;

        public virtual bool CanShow => status >= STA_DISABLED;

        public virtual bool IsEnabled => status == STA_ENABLED;

        public virtual bool CanWork => status >= STA_ENABLED;

        public virtual bool IsPreferred => status == STA_PREFERRED;
    }
}