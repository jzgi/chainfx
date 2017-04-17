namespace Greatbone.Core
{
    public class StrContent : DynamicContent
    {
        public StrContent(bool octal, bool pooled = false, int capacity = 256) : base(octal, pooled, capacity)
        {
        }

        public override string Type => "text/plain";
    }
}