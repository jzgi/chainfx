using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
    ///
    /// The processing of an queued message. 
    ///
    public class MsgContext : IDisposable
    {
        private string topic;

        private string key;

        internal object msg;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}