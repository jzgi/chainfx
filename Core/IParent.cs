namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebDo> Children { get; }

        T AddChild<T>(string key, object state = null) where T : WebDo;

        string Folder { get; }

    }

}