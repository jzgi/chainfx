namespace ChainFX.Web;

/// <summary>
/// A cloud side event poll procedure for pomping both incoming and outgoing events periodically.
/// </summary>
public interface IPollable
{
    /// <summary>
    /// The event input/output polling procedure.
    /// </summary>
    /// <param name="wc"></param>
    void @event(WebContext wc);
}