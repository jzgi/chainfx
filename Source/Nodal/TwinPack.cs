namespace ChainFx.Nodal;

public abstract class TwinPack<E> 
    where E : IKeyable<short>
{
    private Map<short, E> entries;
}