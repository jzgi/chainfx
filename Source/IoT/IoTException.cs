using System;

namespace SkyCloud.IoT
{
    ///
    /// Thrown to indicate a chain-related error.
    ///
    public class IoTException : Exception
    {
        /// <summary>
        /// The status code.
        /// </summary>
        public int Code { get; internal set; }

        public IoTException()
        {
        }

        public IoTException(string message) : base(message)
        {
        }

        public IoTException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}