namespace Greatbone.Core
{
    public class HtmlContent : DynamicContent
    {
        public HtmlContent(int capacity) : base(capacity)
        {
        }

        public HtmlContent(byte[] buffer, int count) : base(buffer, count)
        {
        }

        public override string Type => "text/html";

    }
}