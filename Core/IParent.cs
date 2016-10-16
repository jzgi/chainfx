namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebSub> Subs { get; }

        T AddSub<T>(string key, bool authreq) where T : WebSub;

        string Folder { get; }

    }
}