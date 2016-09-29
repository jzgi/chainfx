using System;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents a data object that is compliant to standard data exchange mechanisms.
    /// </summary>
    public class JsonException : Exception
    {
        public JsonException(string msg) : base(msg) { }
    }
}