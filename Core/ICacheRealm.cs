namespace Greatbone.Core
{
    ///
    /// An independent scope of state and data.
    ///
    public interface ICacheRealm
    {
        ///
        /// Cache-Control and Last-Modified and If-Modified-Since
        long ModifiedOn { get; set; }
    }
}