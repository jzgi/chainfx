using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Map : Map<string>
    {
        public static readonly Map<short>

            PickShort = new Map<short> { Pick = true },

            PickShortPlus = new Map<short> { Pick = true, Plus = true };

        public static readonly Map<int>

            PickInt = new Map<int> { Pick = true },

            PickIntPlus = new Map<int> { Pick = true, Plus = true };

        public static readonly Map<long>

            PickLong = new Map<long> { Pick = true },

            PickLongPlus = new Map<long> { Pick = true, Plus = true };

        public static readonly Map<string>

            PickString = new Map<string> { Pick = true },

            PickStringPlus = new Map<string> { Pick = true, Plus = true };

        public Map(int capacity = 8) : base(capacity)
        {
        }
    }

    public class Map<K> : Dictionary<K, string>
    {
        public Map(int capacity = 8) : base(capacity)
        {
        }

        public bool Pick { get; set; }

        public bool Plus { get; set; }

        public bool Large { get; set; }
    }
}