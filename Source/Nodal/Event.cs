using System;

namespace ChainFx.Nodal;

/// <summary>
/// An edgly internet event.
/// </summary>
public struct Event : IKeyable<short>
{
    private short typ;

    private DateTime start;

    private DateTime end;

    private object content;

    public short Key => typ;
}