using System;

namespace ChainFx.Nodal;

public interface ITwin<out S> : IKeyable<int>
    where S : IEquatable<S>, IComparable<S>
{
    S SetKey { get; }
}