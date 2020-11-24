namespace SkyChain.Chain
{
    public class ChainFlow : IKeyable<short>
    {
        readonly short typ;

        readonly string name;

        readonly ChainActivity[] activities;

        readonly short size;

        public ChainFlow(short typ, string name, ChainActivity[] activities)
        {
            this.typ = typ;
            this.name = name;
            this.activities = activities;
            this.size = (short) activities.Length;

            // init each steps
            for (short i = 0; i < size; i++)
            {
                var a = activities[i];
                // set contextual
                a.flow = this;
                a.step = (short) (i + 1);
                // call custom initializer
                a.OnDefine();
            }
        }

        public short Key => typ;

        public short Typ => typ;

        public string Name => name;

        public ChainActivity[] Activities => activities;

        public ChainActivity StartActivity => activities[0];

        public ChainActivity GetActivity(short step) => activities[step];

        public short Size => size;
    }
}