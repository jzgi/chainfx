using System;

namespace Greatbone
{
    /// <summary>
    /// To indicate that a content parsing-related exception occured.
    /// </summary>
    public class ParserException : Exception
    {
        public ParserException(string msg) : base(msg)
        {
        }
    }
}