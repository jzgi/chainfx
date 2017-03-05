using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Dict : Dictionary<string, string>
    {
        public Dict(int capacity = 8) : base(capacity)
        {
        }
    }
}