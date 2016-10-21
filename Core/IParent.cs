namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebControl> Children { get; }

        T AddChild<T>(string key, object state = null) where T : WebControl;

        string Folder { get; }

    }

}