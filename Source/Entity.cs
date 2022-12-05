using System;

namespace ChainFx
{
    /// <summary>
    /// A data model that represents a general entity object.
    /// </summary>
    public abstract class Entity : IData
    {
        public const short
            STA_VOID = 0,
            STA_NORMAL = 1,
            STA_TOP = 2;

        public const short
            STU_CREATED = 0,
            STU_ADAPTED = 1,
            STU_OKED = 2,
            STU_DONE = 4;

        public static readonly Map<short, string> States = new Map<short, string>
        {
            {STA_VOID, "停用"},
            {STA_NORMAL, "正常"},
            {STA_TOP, "置顶"},
        };

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STU_CREATED, "新建"},
            {STU_ADAPTED, "接纳"},
            {STU_OKED, "处理"},
            {STU_DONE, "结束"},
        };


        public const short
            MSK_ID = 0x0001,
            MSK_TYP = 0x0002,
            MSK_BORN = 0x0004,
            MSK_EDIT = 0x0008,
            MSK_LATER = 0x0010,
            MSK_EXTRA = 0x0100;


        public short typ;
        public short state;
        public string name;
        public string tip;
        public DateTime created;
        public string creator;
        public DateTime adapted;
        public string adapter;
        internal string oker;
        internal DateTime oked;
        public short status;


        public virtual void Read(ISource s, short msk = 0xff)
        {
            if ((msk & MSK_TYP) == MSK_TYP || (msk & MSK_BORN) == MSK_BORN)
            {
                s.Get(nameof(typ), ref typ);
            }
            if ((msk & MSK_BORN) == MSK_BORN)
            {
                s.Get(nameof(created), ref created);
                s.Get(nameof(creator), ref creator);
            }
            if ((msk & MSK_EDIT) == MSK_EDIT)
            {
                s.Get(nameof(state), ref state);
                s.Get(nameof(name), ref name);
                s.Get(nameof(tip), ref tip);
                s.Get(nameof(adapted), ref adapted);
                s.Get(nameof(adapter), ref adapter);
            }
            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Get(nameof(oker), ref oker);
                s.Get(nameof(oked), ref oked);
                s.Get(nameof(status), ref status);
            }
        }

        public virtual void Write(ISink s, short msk = 0xff)
        {
            if ((msk & MSK_BORN) == MSK_BORN)
            {
                s.Put(nameof(typ), typ);
                s.Put(nameof(created), created);
                s.Put(nameof(creator), creator);
            }
            if ((msk & MSK_EDIT) == MSK_EDIT)
            {
                s.Put(nameof(state), state);
                s.Put(nameof(name), name);
                s.Put(nameof(tip), tip);
                s.Put(nameof(adapted), adapted);
                s.Put(nameof(adapter), adapter);
            }
            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Put(nameof(oker), oker);
                s.Put(nameof(oked), oked);
                s.Put(nameof(status), status);
            }
        }

        public virtual string Tip => tip;

        public virtual bool IsDisabled => state == STA_VOID;

        public virtual bool IsEnabled => state == STA_NORMAL;

        public virtual bool IsWorkable => state >= STA_NORMAL;

        public virtual bool IsTop => state == STA_TOP;
    }
}