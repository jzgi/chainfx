namespace ChainFx.Web
{
    /// <summary>
    /// A comment tag for generating reference documentation. 
    /// </summary>
    public interface IDocTag
    {
        public const string CRLF = "\r\n";

        void Render(HtmlBuilder h);
    }
}