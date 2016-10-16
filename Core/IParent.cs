namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebSub> Subs { get; }

        string Folder { get; }

    }
}