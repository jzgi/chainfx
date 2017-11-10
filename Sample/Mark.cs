namespace Greatbone.Sample
{
    public class Mark
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
    }
}