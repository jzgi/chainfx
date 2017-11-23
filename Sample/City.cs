using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// A city data object.
    /// </summary>
    public class City : IData
    {
        public const short LOWER = 2;

        public static Map<string, City> All;

        public static Area[] AreasOf(string city) => city == null ? null : All[city]?.areas;

        internal string name;

        internal double x1, y1, x2, y2;

        internal Area[] areas;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(x1), ref x1);
            i.Get(nameof(y1), ref y1);
            i.Get(nameof(x2), ref x2);
            i.Get(nameof(y2), ref y2);
            if ((proj & LOWER) == LOWER)
            {
                i.Get(nameof(areas), ref areas);
            }
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(x1), x1);
            o.Put(nameof(y1), y1);
            o.Put(nameof(x2), x2);
            o.Put(nameof(y2), y2);
            if ((proj & LOWER) == LOWER)
            {
                o.Put(nameof(areas), areas);
            }
        }

        public static City FindCity(string city)
        {
            return city == null ? null : All[city];
        }

        public Area FindArea(string area)
        {
            if (areas != null)
            {
                for (int i = 0; i < areas.Length; i++)
                {
                    if (areas[i].name == area)
                    {
                        return areas[i];
                    }
                }
            }
            return default;
        }

        public Area[] Areas => areas;

        public override string ToString()
        {
            return name;
        }
    }

    public struct Area : IData
    {
        internal string name;

        internal double x1, y1, x2, y2;

        internal string[] places;

        public void Read(IDataInput i, short proj = 0x00ff)
        {
            i.Get(nameof(name), ref name);
            i.Get(nameof(x1), ref x1);
            i.Get(nameof(y1), ref y1);
            i.Get(nameof(x2), ref x2);
            i.Get(nameof(y2), ref y2);
            i.Get(nameof(places), ref places);
        }

        public void Write<R>(IDataOutput<R> o, short proj = 0x00ff) where R : IDataOutput<R>
        {
            o.Put(nameof(name), name);
            o.Put(nameof(x1), x1);
            o.Put(nameof(y1), y1);
            o.Put(nameof(x2), x2);
            o.Put(nameof(y2), y2);
            o.Put(nameof(places), places);
        }

        public override string ToString()
        {
            return name;
        }
    }
}