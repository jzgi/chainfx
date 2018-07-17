using System;
using Greatbone;

namespace Samp
{
    /// <summary>
    /// A purchase order data model.
    /// </summary>
    public class Po : IData
    {
        public static readonly Tut Empty = new Tut();

        public const byte ID = 1;

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "新建"},
            {1, "发布"},
        };

        internal short id;
        internal string subject;
        internal string text;
        internal DateTime created;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(subject), ref subject);
            s.Get(nameof(text), ref text);
            s.Get(nameof(created), ref created);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(subject), subject);
            s.Put(nameof(text), text);
            s.Put(nameof(created), created);
            s.Put(nameof(status), status);
        }
    }
}