using Greatbone;

namespace Core
{
    /// <summary>
    /// A city data object.
    /// </summary>
    public class City : IData, IKeyable<string>
    {
        public static City[] All;

        internal string id;
        internal string name;
        internal double x1, y1, x2, y2;

        public void Read(ISource s, byte proj = 0x0f)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(x1), ref x1);
            s.Get(nameof(y1), ref y1);
            s.Get(nameof(x2), ref x2);
            s.Get(nameof(y2), ref y2);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(x1), x1);
            s.Put(nameof(y1), y1);
            s.Put(nameof(x2), x2);
            s.Put(nameof(y2), y2);
        }

        public string Key => id;

        public override string ToString()
        {
            return name;
        }
    }
}