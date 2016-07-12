namespace Greatbone.Core
{
    public interface IToken
    {
        bool Can(string zone, int role);
    }
}