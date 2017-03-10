using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Map : Dictionary<string, string>
    {
        public Map(int capacity = 8) : base(capacity)
        {
        }
    }

    public class Map<D> : Dictionary<string, D> where D : IData
    {
        public Map(int capacity = 8) : base(capacity)
        {
        }
    }

    public class Set<K> : Dictionary<K, string>
    {
        public Set(int capacity = 8) : base(capacity)
        {
        }
    }
}