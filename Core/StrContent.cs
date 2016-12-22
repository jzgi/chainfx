namespace Greatbone.Core
{

    public class StrContent : DynamicContent
    {
        const int InitialCapacity = 256;

        public StrContent(bool binary, bool pooled, int capacity = InitialCapacity) : base(binary, pooled, capacity)
        {
        }

        public override string MimeType => "text/plain";
    }
}