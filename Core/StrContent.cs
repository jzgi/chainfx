namespace Greatbone.Core
{

    public class StrContent : DynamicContent
    {
        const int InitialCapacity = 256;

        string text;

        public StrContent(bool binary, bool pooled, int capacity = InitialCapacity) : base(binary, pooled, capacity)
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
        
        public override string CType => "text/plain";

    }

}