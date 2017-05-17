namespace Greatbone.Core
{
    public class StrContent : DynamicContent
    {
        public StrContent(bool octet, bool pooled = false, int capacity = 256) : base(octet, capacity)
        {
        }

        public override string Type => "text/plain";
    }
}