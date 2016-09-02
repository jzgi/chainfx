namespace Greatbone.Core
{
    public class HtmlContent : DynamicContent
    {
        public HtmlContent(byte[] buffer) : base(buffer)
        {
        }

        public HtmlContent(byte[] buffer, int count) : base(buffer, count)
        {
        }
    }
}