using System;

namespace CloudUn.Net
{
    public class Typ : IData
    {
        internal short id;
        
        internal string remark;
        
        internal bool r;
        
        internal bool w;
        
        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
        }

        public void Write(ISink s, byte proj = 15)
        {
        }
    }
}