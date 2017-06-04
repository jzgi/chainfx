namespace Greatbone.Core
{
    public struct Dual
    {
        public Dual(string x, string y)
        {
            X = x;
            Y = y;
        }

        public string X { get; internal set; }

        public string Y { get; internal set; }
    }

    public struct Dual<TX, TY>
    {
        public Dual(TX x, TY y)
        {
            X = x;
            Y = y;
        }

        public TX X { get; internal set; }

        public TY Y { get; internal set; }
    }

    public struct Triple
    {
        public Triple(string x, string y, string z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public string X { get; internal set; }

        public string Y { get; internal set; }

        public string Z { get; internal set; }
    }

    public struct Triple<TX, TY, TZ>
    {
        public Triple(TX x, TY y, TZ z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public TX X { get; internal set; }

        public TY Y { get; internal set; }

        public TZ Z { get; internal set; }
    }

}