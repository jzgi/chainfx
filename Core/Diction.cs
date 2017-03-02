using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Diction : Dictionary<string, string>
    {
        public Diction(int capacity = 8) : base(capacity)
        {
        }
    }
}