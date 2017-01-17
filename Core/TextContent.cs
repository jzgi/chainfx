namespace Greatbone.Core
{
    public class TextContent : DynamicContent
    {
        public TextContent(bool sendable, bool pooled, int capacity = 256) : base(sendable, pooled, capacity)
        {
        }

        public override string Type => "text/plain";
    }
}