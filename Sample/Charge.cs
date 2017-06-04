using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public class Charge : IData
    {
        public static readonly Item Empty = new Item();

        public const short CREATED = 0, COMMITED = 2, ACCEPTED = 3, SETTLED = 4;

        // status
        static readonly Opt<short> STATUS = new Opt<short>
        {
            [CREATED] = "新建",
            [COMMITED] = "已提交/待受理",
            [ACCEPTED] = "已受理",
            [SETTLED] = "已解决",
        };

        internal int id;
        internal string wx;
        internal string nickname;
        internal string shopid;
        internal string shopname;
        internal string descr;
        internal string unit;
        internal int min; // minimal ordered
        internal int step;
        internal short status;

        public void ReadData(IDataInput i, short proj = 0)
        {
        }

        public void WriteData<R>(IDataOutput<R> o, short proj = 0) where R : IDataOutput<R>
        {
        }
    }
}