namespace Greatbone.Core
{

    public class StrContent : DynamicContent
    {
        const int InitialCapacity = 256;

        public StrContent(bool sendable, bool pooled, int capacity = InitialCapacity) : base(sendable, pooled, capacity)
        {
        }

        public override string MimeType => "text/plain";
    }
}