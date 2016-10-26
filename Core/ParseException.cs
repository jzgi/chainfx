using System;

namespace Greatbone.Core
{
    /// <summary>
    /// To indicate that a content parsing-related exception occured.
    /// </summary>
    public class ParseException : Exception
    {
     
        public ParseException(string message) : base(message) { }
        
    }
}