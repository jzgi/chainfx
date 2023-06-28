using System;

namespace ChainFx.Nodal;

public abstract class TwinPack<E> : Map<short, E>
    where E : IKeyable<short>
{
    private DateTime start;

    private DateTime end;
}