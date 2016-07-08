namespace Greatbone.Core
{
    public delegate bool Checker();

    public delegate bool Checker<Z>() where Z : IZone;
}