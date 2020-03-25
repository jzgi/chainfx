using System;

namespace CloudUn.Net
{
    public class User : IData
    {
        public static readonly User Empty = new User();

        public const byte ID = 1, PRIVACY = 2, LATER = 4, EXTRA = 0x10;

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