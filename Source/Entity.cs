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
            STU_OKED = 4;

        public static readonly Map<short, string> Statuses = new()
        {
            { STU_VOID, null },
            { STU_CREATED, "新建" },
            { STU_ADAPTED, "调整" },
            { STU_OKED, "上线" },
        };


        public const short
            MSK_ID = 0x0001,
            MSK_BORN = 0x0002,
            MSK_TYP = 0x0004,
            MSK_EDIT = 0x0010,
            MSK_LATER = 0x0020,
            MSK_STATUS = 0x0040,
            MSK_AUX = 0x0100,
            MSK_EXTRA = 0x0200;


        public short typ;
        public string name;
        public string tip;

        public DateTime created;
        public string creator;

        public DateTime adapted;
        public string adapter;

        public string oker;
        public DateTime oked;
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
                s.Get(nameof(name), ref name);
                s.Get(nameof(tip), ref tip);
                s.Get(nameof(adapted), ref adapted);
                s.Get(nameof(adapter), ref adapter);
            }

            if ((msk & MSK_STATUS) == MSK_STATUS || (msk & MSK_LATER) == MSK_LATER)
            {
                s.Get(nameof(status), ref status);
            }

            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Get(nameof(oker), ref oker);
                s.Get(nameof(oked), ref oked);
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

            if ((msk & MSK_STATUS) == MSK_STATUS || (msk & MSK_LATER) == MSK_LATER)
            {
                s.Put(nameof(status), status);
            }

            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Put(nameof(oker), oker);
                s.Put(nameof(oked), oked);
            }
        }

        public virtual string Tip => tip;

        public bool IsVoid => status == STU_VOID;

        public bool IsEnabled => status > STU_VOID;

        public bool IsModifiable => status < STU_OKED;

        public bool IsOked => status == STU_OKED;

        public short Status => status;

        public virtual short ToState() => 0;

        public override string ToString() => name;
    }
}