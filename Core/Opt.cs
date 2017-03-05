using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Opt<K> : Dictionary<K, string>
    {
        public Opt(int capacity = 8) : base(capacity)
        {
        }
    }
}