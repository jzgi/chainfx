using System;

namespace SkyChain.Chain
{
    public class TransactDef : IKeyable<short>
    {
        readonly short typ;

        readonly string name;

        readonly Activity[] activities;

        readonly short size;

        public TransactDef(short typ, string name, Activity[] activities)
        {
            this.typ = typ;
            this.name = name;
            this.activities = activities;
            this.size = (short) activities.Length;

            // init each steps
            for (short i = 0; i <= size; i++)
            {
                var a = activities[i];
                // set contextual
                a.parent = this;
                a.step = i;
                // call custom initializer
                a.OnDefine();
            }
        }

        public short Key => typ;

        public short Typ => typ;

        public string Name => name;

        public Activity[] Activities => activities;

        public Activity StartActivity => activities[0];

        public Activity GetActivity(short step) => activities[step];

        public short Size => size;
    }
}