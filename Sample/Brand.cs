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

        public void Load(ISource s, uint x = 0)
        {
        }

        public void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>
        {
        }
    }
}