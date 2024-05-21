using ChainFX.Web;

namespace ChainFX.Source;

public interface IExternable
{
    void @extern(WebContext wc);
}