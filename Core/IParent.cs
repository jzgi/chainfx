namespace Greatbone.Core
{
    public interface IParent
    {
        string Key { get; }

        Roll<WebControl> Subs { get; }

        T AddControl<T>(string key, bool authreq) where T : WebControl;

        string Folder { get; }

    }
}