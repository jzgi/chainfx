using System;


namespace Greatbone.Core
{
    ///
    /// The processing of an queued message. 
    ///
    public struct WebEvent : IContext, IDisposable
    {
        readonly WebPeer peer;

        readonly long id;

        readonly string topic;

        readonly string subkey;

        // either Obj or Arr
        readonly object body;

        internal WebEvent(WebPeer peer, long id, string topic, string subkey, object body)
        {
            this.peer = peer;
            this.id = id;
            this.topic = topic;
            this.subkey = subkey;
            this.body = body;
        }

        public long Id => id;

        public string Topic => Topic;

        public string Subkey => subkey;

        public Obj BodyObj => (Obj)body;

        public Arr BodyArr => (Arr)body;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}