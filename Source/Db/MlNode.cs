using System;

namespace SkyChain.Db
{
    public class MlNode : IData, IKeyable<short>
    {
        short id;
        
        int bias;
        
        MlNode[] @in;

        decimal[] weights; // incoming connections


        MlNode[] @out;


        public bool Top => @in == null;

        public bool Bottom => @out == null;

        public void Input()
        {
            throw new NotImplementedException();
        }

        public void Output()
        {
            throw new NotImplementedException();
        }

        public void Read(ISource s, byte proj = 15)
        {
            throw new NotImplementedException();
        }

        public void Write(ISink s, byte proj = 15)
        {
            throw new NotImplementedException();
        }

        public short Key => id;
    }
}