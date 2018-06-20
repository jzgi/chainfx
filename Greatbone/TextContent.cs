namespace Greatbone
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextContent : DynamicContent
    {
        public TextContent(bool bin, bool pooled = false, int capacity = 512) : base(bin, capacity)
        {
        }

        public override string Type => "text/plain";
    }
}