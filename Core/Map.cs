using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A dictionary structure that provides a collection of options to UI elements.
    /// </summary>
    public class Map<K, V> : SortedDictionary<K, V>, IOptable<K>
    {
        public void ForEach(Action<K, object> handler)
        {
            foreach (var pair in this)
            {
                handler(pair.Key, pair.Value);
            }
        }

        public string Obtain(K key)
        {
            return this[key].ToString();
        }
    }
}