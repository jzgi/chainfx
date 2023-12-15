using System;

namespace ChainFX;

/// <summary>
/// An object with an unique key.
/// </summary>
public interface IKeyable<out K> where K : IEquatable<K>, IComparable<K>
{
    K Key { get; }
}