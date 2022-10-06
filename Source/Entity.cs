﻿using System;

namespace ChainFx
{
    /// <summary>
    /// A data model that represents a general entity object.
    /// </summary>
    public abstract class Entity : IData
    {
        public const short
            STU_VOID = 0,
            STU_NORMAL = 1,
            STU_TOP = 2;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {STU_VOID, "停止"},
            {STU_NORMAL, "正常"},
            {STU_TOP, "置顶"},
        };

        public const short
            MSK_ID = 0x0001,
            MSK_TYP = 0x0002,
            MSK_BORN = 0x0004,
            MSK_EDIT = 0x0008,
            MSK_LATER = 0x0010,
            MSK_EXTRA = 0x0100;


        public short typ;
        public short status;
        public string name;
        public string tip;
        public DateTime created;
        public string creator;
        public DateTime adapted;
        public string adapter;

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
                s.Get(nameof(status), ref status);
                s.Get(nameof(name), ref name);
                s.Get(nameof(tip), ref tip);
            }
            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Get(nameof(adapted), ref adapted);
                s.Get(nameof(adapter), ref adapter);
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
                s.Put(nameof(status), status);
                s.Put(nameof(name), name);
                s.Put(nameof(tip), tip);
            }
            if ((msk & MSK_LATER) == MSK_LATER)
            {
                s.Put(nameof(adapted), adapted);
                s.Put(nameof(adapter), adapter);
            }
        }

        public virtual string Tip => tip;

        public virtual bool IsDisabled => status == STU_VOID;

        public virtual bool IsEnabled => status == STU_NORMAL;

        public virtual bool IsWorkable => status >= STU_NORMAL;

        public virtual bool IsTop => status == STU_TOP;
    }
}