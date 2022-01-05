using SkyChain;

namespace SkyChain.Db
{
    public class DbNode : IData, IKeyable<short>
    {
        short id;

        int bias;

        DbNode[] @in;

        decimal[] weights; // incoming connections

        DbNode[] @out;


        public bool Top => @in == null;

        public bool Bottom => @out == null;

        public void Input()
        {
        }

        public void Output()
        {
        }

        public void Read(ISource s, short proj = 0x0fff)
        {
        }

        public void Write(ISink s, short proj = 0x0fff)
        {
        }

        public short Key => id;
    }
}