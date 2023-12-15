using System;

namespace ChainFX.Nodal;

public abstract class DbCache
{
    // actual cached object type 
    readonly Type typ;

    // bitwise mask
    readonly short flag;

    // expiry period in seconds
    readonly int maxage;

    // fetcher delegate, one of the three forms, or null when twin cache 
    protected readonly Delegate fetch;


    protected DbCache(Delegate fetch, Type typ, int maxage, short flag)
    {
        this.fetch = fetch;
        this.typ = typ;
        this.maxage = maxage;
        this.flag = flag;
    }


    public Delegate Fetch => fetch;

    public virtual Type Typ => typ;

    public abstract bool IsAsync { get; }

    public int MaxAge => maxage;

    public short Flag => flag;
}