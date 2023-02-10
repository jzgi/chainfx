using System;

namespace ChainFx
{
    /// <summary>
    /// A data model that represents a general entity object.
    /// </summary>
    public abstract class Entity : IData
    {
        public const short
            STU_VOID = 0,
            STU_CREATED = 1,
            STU_ADAPTED = 2,
            STU_OKED = 4,
            STU_ABORTED = 8;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STU_VOID, null},
            {STU_CREATED, "新建"},
            {STU_ADAPTED, "处理"},
            {STU_OKED, "完成"},
            {STU_ABORTED, "撤销"},
        };


        public const short
            MSK_ID = 0x0001,
            MSK_TYP = 0x0002,
            MSK_BORN = 0x0004,
            MSK_EDIT = 0x0008,
            MSK_LATER = 0x0010,
            MSK_EXTRA = 0x0100;


        public short typ;
        public string name;
        public string tip;

        public DateTime created;
        public string creator;

        public DateTime adapted;
        public string adapter;

        public string ender;
        public DateTime ended;
        public short status;
        public short state;


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
                s.Get(nameof(name), ref name);
                s.Get(nameof(tip), ref tip);
                s.Get(nameof(adapted), ref adapted);
                s.Get(nameof(adapter), ref adapter);
            }
            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Get(nameof(ender), ref ender);
                s.Get(nameof(ended), ref ended);
                s.Get(nameof(status), ref status);
                s.Get(nameof(state), ref state);
            }
        }

        public virtual void Write(ISink s, short msk = 0xff)
        {
            if ((msk & MSK_TYP) == MSK_TYP || (msk & MSK_BORN) == MSK_BORN)
            {
                s.Put(nameof(typ), typ);
            }
            if ((msk & MSK_BORN) == MSK_BORN)
            {
                s.Put(nameof(created), created);
                s.Put(nameof(creator), creator);
            }
            if ((msk & MSK_EDIT) == MSK_EDIT)
            {
                s.Put(nameof(name), name);
                s.Put(nameof(tip), tip);
                s.Put(nameof(adapted), adapted);
                s.Put(nameof(adapter), adapter);
            }
            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Put(nameof(ender), ender);
                s.Put(nameof(ended), ended);
                s.Put(nameof(status), status);
                s.Put(nameof(state), state);
            }
        }

        public string Tip => tip;

        public bool IsDisabled => status == STU_VOID || status == STU_ABORTED;

        public bool IsEnabled => state > STU_VOID && status < STU_ABORTED;

        public bool IsChangeable => status < STU_OKED;

        public bool IsEnded => status >= STU_OKED;

        public bool IsCancelled => status == STU_ABORTED;

        public override string ToString() => name;
    }
}