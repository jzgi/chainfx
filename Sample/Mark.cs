using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Mark : IData
    {
        public static Mark[] All = new Mark[]
        {
            new Mark("粗粮杂粮", "", ""),
            new Mark("自产", "", ""),
            new Mark("冷压粗磨", "", ""),
            new Mark("非转基因", "", ""),
        };

        internal string mark;

        internal string icon;

        internal string descr;

        public Mark(string mark, string icon, string descr)
        {
            this.mark = mark;
            this.icon = icon;
            this.descr = descr;
        }

        public void Read(IDataInput i, short proj = 255)
        {
        }

        public void Write<R>(IDataOutput<R> o, short proj = 255) where R : IDataOutput<R>
        {
        }
    }
}