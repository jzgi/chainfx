using System;

namespace Greatbone.Web
{
    ///
    /// Thrown to indicate an illegal structuring in a work hierarchy.
    ///
    public class WebException : Exception
    {
        /// <summary>
        /// The status code.
        /// </summary>
        public int Code { get; internal set; }

        public WebException()
        {
        }

        public WebException(string message) : base(message)
        {
        }

        public WebException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}