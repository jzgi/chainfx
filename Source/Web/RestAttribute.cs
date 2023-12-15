using System;

namespace ChainFX.Web
{
    /// <summary>
    /// A comment tag for generating RESTful API documentation. 
    /// </summary>
    public abstract class RestAttribute : Attribute
    {
        public const string CRLF = "\r\n";

        public abstract void Render(HtmlBuilder h);
    }
}