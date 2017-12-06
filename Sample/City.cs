using Greatbone.Core;

namespace Greatbone.Samp
{
    /// <summary>
    /// A city data object.
    /// </summary>
    public class City : IData
    {
        public static Map<string, City> All;

        public static Area[] AreasOf(string city) => city == null ? null : All[city]?.areas;

        public static string[] SitesOf(string city, string area)
        {
            var areas = AreasOf(city);
            if (areas != null)
            {
                for (int i = 0; i < areas.Length; i++)
                {
                    if (areas[i].name == area) return areas[i].sites;
                }
            }
            return null;
        }

        internal string name;

        internal Area[] areas;

        internal double x1, y1, x2, y2;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(x1), ref x1);
            i.Get(nameof(y1), ref y1);
            i.Get(nameof(x2), ref x2);
            i.Get(nameof(y2), ref y2);
            i.Get(nameof(areas), ref areas);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(x1), x1);
            o.Put(nameof(y1), y1);
            o.Put(nameof(x2), x2);
            o.Put(nameof(y2), y2);
            o.Put(nameof(areas), areas);
        }

        public override string ToString()
        {
            return name;
        }
    }

    public struct Area : IData
    {
        internal string name;

        internal string[] sites;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(sites), ref sites);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(sites), sites);
        }

        public override string ToString()
        {
            return name;
        }
    }
}