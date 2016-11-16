namespace Greatbone.Core
{
    public class BootStrapContent : HtmlContent
    {
        public BootStrapContent(bool raw, bool pooled, int capacity = 8192) : base(raw, pooled, capacity)
        {
        }
    }
}