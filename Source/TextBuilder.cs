namespace ChainFx
{
    /// <summary>
    /// To generate a plain/text string or CSV content. 
    /// </summary>
    public class TextBuilder : ContentBuilder
    {
        public TextBuilder(bool bytely, int capacity) : base(bytely, capacity) {}

        public override string CType { get; set; } = "text/plain";
    }
}