using System;

namespace ChainFX;

public interface IGroupable<K> : IKeyable<K> where K : IEquatable<K>, IComparable<K>
{
    /// <summary>
    /// To determine whether the same group as the specified key. 
    /// </summary>
    /// <param name="akey"></param>
    /// <returns></returns>
    bool IsSameGroupAs(K akey);
}