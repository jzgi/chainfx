using System;

namespace ChainFx
{
    public interface IGroupable<K> : IKeyable<K>
        where K : IEquatable<K>, IComparable<K>
    {
        /// <summary>
        /// To determine if it is in the same group as the given object. 
        /// </summary>
        /// <param name="akey"></param>
        /// <returns></returns>
        bool GroupWith(K akey);
    }
}