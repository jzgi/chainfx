namespace Greatbone.Core
{
    public struct Dual<TX, TY>
    {
        readonly TX x;

        readonly TY y;

        public Dual(TX x, TY y)
        {
            this.x = x;
            this.y = y;
        }

        public TX X => x;

        public TY Y => y;
    }

    public struct Triple<TX, TY, TZ>
    {
        readonly TX x;

        readonly TY y;

        readonly TZ z;

        public Triple(TX x, TY y, TZ z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public TX X => x;

        public TY Y => y;

        public TZ Z => z;
    }
}