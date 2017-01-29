using System;

namespace Greatbone.Core
{
    ///
    /// An object capable of being a roll structure member.
    ///
    public interface IKeyed<K> where K : IComparable<K>
    {
        K Key { get; }
    }
}