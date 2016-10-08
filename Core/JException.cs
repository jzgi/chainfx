using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To indicate that a JSON-related exception occured.
    /// </summary>
    public class JException : Exception
    {
        public JException(string message) : base(message) { }
    }
}