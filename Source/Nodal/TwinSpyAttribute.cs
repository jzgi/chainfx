using System;

namespace ChainFX.Nodal;

/// <summary>
/// To bind notice functionality to action.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public abstract class TwinSpyAttribute : Attribute
{
    protected readonly short slot;

    protected TwinSpyAttribute(short slot)
    {
        this.slot = slot;
    }

    public short Slot => slot;

    public abstract int Do(int twinKey, bool clear = false);
}