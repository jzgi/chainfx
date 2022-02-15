namespace SkyChain
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextContent : DynamicContent
    {
        public TextContent(bool bytel, int capacity) : base(bytel, capacity) {}

        public override string Type { get; set; } = "text/plain";
    }
}