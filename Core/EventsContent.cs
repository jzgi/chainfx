using System;

namespace Greatbone.Core
{
    /// 
    /// To generate an pack of events.
    /// 
    public class EventsContent : DynamicContent
    {
        const int InitialCapacity = 32 * 1024;

        public EventsContent(bool pooled, int capacity = InitialCapacity) : base(true, pooled, capacity)
        {
        }

        public override string MimeType => "application/events";

        public void Add(long id, string name, DateTime time, object body)
        {

        }
    }
}