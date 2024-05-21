using System;

namespace ChainFX.Web;

/// <summary>
/// To bind notice functionality to action.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public abstract class WatchAttribute : Attribute
{
    readonly short kind;

    protected WatchAttribute(short kind)
    {
        this.kind = kind;
    }

    public short Kind => kind;

    public abstract int Peek(int key, bool clear = false);
}