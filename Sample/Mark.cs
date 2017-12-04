using Greatbone.Core;

namespace Greatbone.Samp
{
    public class Mark : IData
    {
        public static Mark[] All =
        {
            new Mark("粗粮杂粮",
                ""
            ),
            new Mark("自产", ""),
            new Mark("冷压粗磨", ""),
            new Mark("非转基因", ""),
        };

        internal string name;

        internal string descr;

        public Mark(string name, string descr)
        {
            this.name = name;
            this.descr = descr;
        }

        public void Read(IDataInput i, short proj = 255)
        {
        }

        public void Write<R>(IDataOutput<R> o, short proj = 255) where R : IDataOutput<R>
        {
        }

        public override string ToString() => name;
    }
}