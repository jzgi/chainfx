namespace Greatbone.Core
{
    public class HtmlContent : DynamicContent
    {
        public HtmlContent(int capacity) : base(capacity)
        {
        }

        public override string Type => "text/html";

    }
}