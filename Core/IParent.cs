namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebWork> Children { get; }

        T AddChild<T>(string key, object state = null) where T : WebWork;

        string Folder { get; }

    }

}