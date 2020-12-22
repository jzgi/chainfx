namespace SkyChain
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextContent : DynamicContent
    {
        public TextContent(bool octet, int capacity) : base(octet, capacity) {}

        public override string Type { get; set; } = "text/plain";
    }
}