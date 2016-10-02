namespace Greatbone.Core
{
    public class XmlContent : DynamicContent
    {
        public XmlContent(int capacity) : base(capacity)
        {
        }

        public override string Type => "application/xml";

    }
}