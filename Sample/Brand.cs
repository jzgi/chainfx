using Greatbone.Core;

namespace Greatbone.Sample
{

    public class Brand : IPersist
    {
        public string Id;
        public string Name;
        public string Credential { get; set; }
        public long ModifiedOn { get; set; }

        public string Key => Id;

        public void Load(ISource s, byte x = 0)
        {
        }

        public void Dump<R>(ISink<R> s, byte x = 0) where R : ISink<R>
        {
        }

    }

}