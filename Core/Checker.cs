namespace Greatbone.Core
{
    public delegate bool Checker(IToken token);

    public delegate bool Checker<in TZone>(IToken token, TZone zone) where TZone : IUnit;
}