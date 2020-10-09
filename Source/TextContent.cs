namespace Skyiah
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextContent : DynamicContent
    {
        public TextContent(int capacity) : base(capacity)
        {
        }

        public override string Type { get; set; } = "text/plain";
    }
}