namespace Greatbone.Core
{
    ///
    /// An independent scope of state and data.
    public interface ICacheControl
    {
        ///
        /// Cache-Control and Last-Modified and If-Modified-Since
        long ModifiedOn { get; set; }
    }
}