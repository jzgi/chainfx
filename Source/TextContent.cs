namespace SkyChain
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextContent : DynamicContent
    {
        public TextContent(bool binary, int capacity) : base(binary, capacity) {}

        public override string Type { get; set; } = "text/plain";
    }
}