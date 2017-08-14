using System;

namespace Greatbone.Core
{
    public interface IOptable<K> where K : IEquatable<K>
    {
        void ForEach(Action<K, object> handler);

        string Obtain(K key);
    }
}