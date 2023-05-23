using System;

namespace ChainFx
{
    /// <summary>
    /// An object with an key name so that can be a map element.
    /// </summary>
    public interface IKeyable<out K>
        where K : IEquatable<K>, IComparable<K>
    {
        K Key { get; }
    }
}