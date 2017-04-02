using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A dictionary structure that provides a collection of options to UI elements.
    /// </summary>
    public class Opt<K> : Dictionary<K, string>
    {
        public Opt(int capacity = 8) : base(capacity)
        {
        }

        public int Dialog { get; set; }
    }
}