namespace ChainFx.Web
{
    /// <summary>
    /// A comment tag for documentation. 
    /// </summary>
    public interface IRestfulTag
    {
        public const string CRLF = "\r\n";

        void Render(HtmlBuilder h);
    }
}