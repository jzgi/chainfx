using System;

namespace Greatbone.Core
{
    public class WebException : Exception
    {
        public WebException() { }

        public WebException(string message) : base(message) { }
        
        public WebException(string message, System.Exception inner) : base(message, inner) { }

    }
}