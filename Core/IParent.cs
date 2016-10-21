namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebControl> Controls { get; }

        T AddControl<T>(string key, object state = null) where T : WebControl;

        string Folder { get; }

    }

}