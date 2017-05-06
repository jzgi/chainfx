namespace Greatbone.Core
{
    public struct Value<TX, TY>
    {
        readonly TX x;

        readonly TY y;

        public Value(TX x, TY y)
        {
            this.x = x;
            this.y = y;
        }

        public TX X => x;

        public TY Y => y;
    }

    public struct Value<TX, TY, TZ>
    {
        readonly TX x;

        readonly TY y;

        readonly TZ z;

        public Value(TX x, TY y, TZ z)
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