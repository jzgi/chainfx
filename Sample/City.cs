using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A city data object.
    /// </summary>
    public class City : IData
    {
        public static Map<string, City> All;

        internal string name;
        internal double x1, y1, x2, y2;
        internal string[] distrs;
        internal Area[] areas;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(x1), ref x1);
            i.Get(nameof(y1), ref y1);
            i.Get(nameof(x2), ref x2);
            i.Get(nameof(y2), ref y2);
            i.Get(nameof(distrs), ref distrs);
            i.Get(nameof(areas), ref areas);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(x1), x1);
            o.Put(nameof(y1), y1);
            o.Put(nameof(x2), x2);
            o.Put(nameof(y2), y2);
            o.Put(nameof(distrs), distrs);
            o.Put(nameof(areas), areas);
        }

        public string[] Distrs => distrs;

        public Area[] Areas => areas;

        public bool Contains(double x, double y)
        {
            return true;
        }

        public Area LocateArea(double x, double y)
        {
            return default(Area);
        }

        public override string ToString()
        {
            return name;
        }
    }

    public struct Area : IData
    {
        internal string name;
        internal double x1, y1, x2, y2;
        internal string code;
        internal string[] sites;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(x1), ref x1);
            i.Get(nameof(y1), ref y1);
            i.Get(nameof(x2), ref x2);
            i.Get(nameof(y2), ref y2);
            i.Get(nameof(code), ref code);
            i.Get(nameof(sites), ref sites);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(x1), x1);
            o.Put(nameof(y1), y1);
            o.Put(nameof(x2), x2);
            o.Put(nameof(y2), y2);
            o.Put(nameof(code), code);
            o.Put(nameof(sites), sites);
        }

        public override string ToString()
        {
            return name;
        }
    }
}