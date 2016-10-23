using System;


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