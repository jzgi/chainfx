using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents a JSON-related exception occured.
    /// </summary>
    public class FormatException : Exception
    {
        public FormatException(string message) : base(message) { }
    }
}