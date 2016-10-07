namespace Greatbone.Core
{
    public class HtmContent : DynamicContent
    {
        public HtmContent(int capacity) : base(capacity)
        {
        }

        public override string Type => "text/html";

    }
}