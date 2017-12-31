namespace Greatbone.Core
{
    /// <summary>
    /// To generate a plain/text string. 
    /// </summary>
    public class StrContent : DynamicContent
    {
        public StrContent(bool octet, bool pooled = false, int capacity = 512) : base(octet, capacity)
        {
        }

        public override string Type => "text/plain";
    }
}