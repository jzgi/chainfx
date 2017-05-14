namespace Greatbone.Core
{
    public struct Duo<TX, TY>
    {
        readonly TX x;

        readonly TY y;

        public Duo(TX x, TY y)
        {
            this.x = x;
            this.y = y;
        }

        public TX X => x;

        public TY Y => y;
    }
}