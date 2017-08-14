using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public interface IOptable<K> where K : IEquatable<K>
    {
        ICollection<K> GetKeys();

        string GetText(K key);
    }
}