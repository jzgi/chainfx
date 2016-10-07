using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents a JSON-related exception occured.
    /// </summary>
    public class JException : Exception
    {
        public JException(string message) : base(message) { }
    }
}