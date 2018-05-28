using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// An infor article data object.
    /// </summary>
    public class Inf : IData, IKeyable<short>
    {
        public static readonly Inf Empty = new Inf();

        public const byte ID = 1;

        public const int CREATED = 0, OPEN = 2;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {CREATED, "新建"},
            {OPEN, "发布"},
        };

        internal short id;
        internal string subject;
        internal string text;
        internal DateTime created;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Get(nameof(id), ref id);
            }
            s.Get(nameof(subject), ref subject);
            s.Get(nameof(text), ref text);
            s.Get(nameof(created), ref created);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & ID) > 0)
            {
                s.Put(nameof(id), id);
            }
            s.Put(nameof(subject), subject);
            s.Put(nameof(text), text);
            s.Put(nameof(created), created);
            s.Put(nameof(status), status);
        }

        public short Key => id;
    }
}