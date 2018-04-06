using Greatbone;

namespace Core
{
    /// <summary>
    /// An episode of lessons data object.
    /// </summary>
    public class Lesson : IData
    {
        internal string zh;
        internal string en;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(zh), ref zh);
            s.Get(nameof(en), ref en);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(zh), zh);
            s.Put(nameof(en), en);
            s.Put(nameof(status), status);
        }
    }
}