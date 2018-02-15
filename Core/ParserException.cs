using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To indicate that a content parsing-related exception occured.
    /// </summary>
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}