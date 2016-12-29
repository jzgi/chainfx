using System;

namespace Greatbone.Core
{
    /// 
    /// To generate an pack of events.
    /// 
    public class EventBagContent : DynamicContent
    {
        const int InitialCapacity = 256 * 1024;

        public EventBagContent(bool pooled, int capacity = InitialCapacity) : base(true, pooled, capacity)
        {
        }

        public override string MimeType => "application/events";

        public void Add(long id, string name, DateTime time, object body)
        {
        }
    }
}