using System;

namespace Greatbone.Core
{
    /// <summary>
    /// An data structure that can provide options.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public interface IOptable<K>
    {
        void ForEach(Action<K, object> handler);

        string Obtain(K key);
    }
}