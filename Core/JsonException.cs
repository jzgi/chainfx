using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents a JSON-related exception occured.
    /// </summary>
    public class JsonException : Exception
    {
        public JsonException(string message) : base(message) { }
    }
}