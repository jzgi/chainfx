using System;

namespace DoChain.Web
{
    ///
    /// Thrown to indicate an illegal structuring in a work hierarchy.
    ///
    public class WebException : Exception
    {
        public static readonly WebException AuthFailed = new WebException("Authorization Failed") {Code = 403};

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