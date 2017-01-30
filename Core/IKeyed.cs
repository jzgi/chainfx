using System;

namespace Greatbone.Core
{
    ///
    /// An object having an identifying key.
    ///
    public interface IKeyed<K> where K : IComparable<K>
    {
        K Key { get; }
    }
}