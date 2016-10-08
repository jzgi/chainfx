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

        public void Load(ISource sc, int x = -1)
        {
        }

        public void Save<R>(ISink<R> sk, int x = -1) where R : ISink<R>
        {
        }
    }
}