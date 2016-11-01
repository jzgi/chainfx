using Greatbone.Core;

namespace Greatbone.Sample
{
    
    public class Brand : IPersist
    {
        public string Id;
        public string Name;
        public char[] Credential { get; set; }
        public long ModifiedOn { get; set; }

        public string Key => Id;

        public void Load(ISource s, byte x = 0xff)
        {
        }

        public void Dump<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>
        {
        }
    
    }

}