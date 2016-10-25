namespace Greatbone.Core
{
    public class StrContent : DynamicContent
    {

        public StrContent(string text) : base(1024)
        {
            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                Add(text[i]);
            }
        }

        public override string Type { get; }

    }

}