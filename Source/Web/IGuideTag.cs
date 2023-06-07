namespace ChainFx.Web
{
    /// <summary>
    /// A comment tag for generating user guide. 
    /// </summary>
    public interface IGuideTag
    {
        public const string CRLF = "\r\n";

        void Render(HtmlBuilder h);
    }
}