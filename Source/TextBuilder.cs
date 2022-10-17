namespace ChainFx
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextBuilder : DynamicBuilder
    {
        public TextBuilder(bool bytel, int capacity) : base(bytel, capacity) {}

        public override string CType { get; set; } = "text/plain";
    }
}