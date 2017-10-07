using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A complaint kick data object.
    /// </summary>
    public class Kick : IData
    {
        public static readonly Kick Empty = new Kick();

        public const short CREATED = 0, COMMITED = 2, RESOLVED = 4;

        // status
        static readonly Map<short, string> STATUS = new Map<short, string>
        {
            [CREATED] = "新建",
            [COMMITED] = "已提交",
            [RESOLVED] = "已解决",
        };

        internal int id;
        internal string wx;
        internal string tel;
        internal string name;
        internal string shopid;
        internal string shopname;
        internal string content;
        internal DateTime committed;
        internal short status;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(id), ref id);
            i.Get(nameof(wx), ref wx);
            i.Get(nameof(tel), ref tel);
            i.Get(nameof(name), ref name);
            i.Get(nameof(shopid), ref shopid);
            i.Get(nameof(shopname), ref shopname);
            i.Get(nameof(content), ref content);
            i.Get(nameof(committed), ref committed);
            i.Get(nameof(status), ref status);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(id), id);
            o.Put(nameof(wx), wx);
            o.Put(nameof(tel), tel);
            o.Put(nameof(name), name);
            o.Put(nameof(shopid), shopid);
            o.Put(nameof(shopname), shopname);
            o.Put(nameof(content), content);
            o.Put(nameof(committed), committed);
            o.Put(nameof(status), status);
        }
    }
}