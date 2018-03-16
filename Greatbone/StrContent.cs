namespace Greatbone
{
    /// <summary>
    /// To generate a plain/text string. 
    /// </summary>
    public class StrContent : DynamicContent
    {
        public StrContent(bool bin, bool pooled = false, int capacity = 512) : base(bin, capacity)
        {
        }

        public override string Type => "text/plain";
    }
}