namespace Greatbone.Core
{
    public struct Dual<TA, TB>
    {
        public Dual(TA a, TB b)
        {
            A = a;
            B = b;
        }

        public TA A { get; set; }

        public TB B { get; set; }
    }

    public struct Triple<TA, TB, TC>
    {
        public Triple(TA a, TB b, TC c)
        {
            A = a;
            B = b;
            C = c;
        }

        public TA A { get; set; }

        public TB B { get; set; }

        public TC C { get; set; }
    }

    public struct Quad<TA, TB, TC, TD>
    {
        public Quad(TA a, TB b, TC c, TD d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public TA A { get; set; }

        public TB B { get; set; }

        public TC C { get; set; }

        public TD D { get; set; }
    }

    public struct Quint<TA, TB, TC, TD, TE>
    {
        public Quint(TA a, TB b, TC c, TD d, TE e)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
        }

        public TA A { get; set; }

        public TB B { get; set; }

        public TC C { get; set; }

        public TD D { get; set; }

        public TE E { get; set; }
    }
}