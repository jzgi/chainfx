using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Flag : IData
    {
        public static Flag[] All =
        {
            new Flag("粗粮杂粮", "", ""),
            new Flag("自产", "", ""),
            new Flag("冷压粗磨", "", ""),
            new Flag("非转基因", "", ""),
        };

        internal string flag;

        internal string icon;

        internal string descr;

        public Flag(string flag, string icon, string descr)
        {
            this.flag = flag;
            this.icon = icon;
            this.descr = descr;
        }

        public void Read(IDataInput i, short proj = 255)
        {
        }

        public void Write<R>(IDataOutput<R> o, short proj = 255) where R : IDataOutput<R>
        {
        }

        public override string ToString() => flag;
    }
}