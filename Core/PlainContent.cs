namespace Greatbone.Core
{

    public class PlainContent : DynamicContent
    {
        const int InitialCapacity = 256;

        string text;

        public PlainContent(bool raw, bool pooled, int capacity = InitialCapacity) : base(raw, pooled, capacity)
        {
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                Add(text);
            }

        }
        public override string Type => "text/plain";

    }

}