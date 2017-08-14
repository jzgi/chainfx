using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// A dictionary structure that provides a collection of options to UI elements.
    /// </summary>
    public class Map<K, V> : Dictionary<K, V>, IOptable<K> where K : IEquatable<K>
    {
        public Map(int capacity = 8) : base(capacity)
        {
        }

        public ICollection<K> GetKeys() => Keys;

        public string GetText(K key)
        {
            return this[key].ToString();
        }
    }
}