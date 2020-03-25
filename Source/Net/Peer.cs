using System;

namespace CloudUn.Net
{
    public class Peer : IData
    {
        internal string id;
        
        internal string name;
        
        internal string raddr;
        
        internal DateTime stamp;
        
        internal short status;
        
        public void Read(ISource s, byte proj = 15)
        {
        }

        public void Write(ISink s, byte proj = 15)
        {
        }
    }
}