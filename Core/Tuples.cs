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
}