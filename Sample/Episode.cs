using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// An episode of lessons data object.
    /// </summary>
    public class Episode : IData
    {
        internal string en; // 01 lesson-name
        internal string zh;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(en), ref en);
            s.Get(nameof(zh), ref zh);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(zh), zh);
            s.Put(nameof(en), en);
        }
    }
}