using System;

namespace ChainFX.Nodal;

/// <summary>
/// To de
/// </summary>
/// <typeparam name="FK">the fork-key type</typeparam>
public interface ITwin<out FK> : IKeyable<int> where FK : IEquatable<FK>, IComparable<FK>
{
    FK ForkKey { get; }
}