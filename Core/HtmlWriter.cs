namespace Greatbone.Core
{
    public class HtmlWriter : ContentWriter
    {
        public HtmlWriter(int capacity) : base(capacity)
        {
        }

        public override string Type => "text/html";

    }
}