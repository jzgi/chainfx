namespace Greatbone.Core
{
    public struct Dual
    {
        public Dual(string x, string y)
        {
            X = x;
            Y = y;
        }

        public string X { get; set; }

        public string Y { get; set; }
    }

    public struct Dual<TX, TY>
    {
        public Dual(TX x, TY y)
        {
            X = x;
            Y = y;
        }

        public TX X { get; set; }

        public TY Y { get; set; }
    }

    public struct Triple
    {
        public Triple(string x, string y, string z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public string X { get; set; }

        public string Y { get; set; }

        public string Z { get; set; }
    }

    public struct Triple<TX, TY, TZ>
    {
        public Triple(TX x, TY y, TZ z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public TX X { get; set; }

        public TY Y { get; set; }

        public TZ Z { get; set; }
    }
}