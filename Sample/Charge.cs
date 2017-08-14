using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public class Charge : IData
    {
        public static readonly Item Empty = new Item();

        public const short CREATED = 0, COMMITED = 2, ACCEPTED = 3, SETTLED = 4;

        // status
        static readonly Map<short, string> STATUS = new Map<short, string>
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
        internal short status;

        public void Read(IDataInput i, ushort proj = 0x00ff)
        {
        }

        public void Write<R>(IDataOutput<R> o, ushort proj = 0x00ff) where R : IDataOutput<R>
        {
        }
    }
}