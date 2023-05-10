using System;

namespace ChainFx.Nodal;

public interface ITwin<out G, out K> : IKeyable<K>
    where G : IEquatable<G>, IComparable<G>
    where K : IEquatable<K>, IComparable<K>
{
    G GroupKey { get; }
}