namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebDoer> Children { get; }

        T AddChild<T>(string key, object state = null) where T : WebDoer;

        string Folder { get; }

    }

}