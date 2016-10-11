namespace Greatbone.Core
{
    public class HtmlContent : DynamicContent
    {
        public HtmlContent(int capacity) : base(capacity)
        {
        }

        public override string Type => "text/html";


        public void AddEsc(string v)
        {
            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c == '<')
                {
                    Add("&lt;");
                }
                else if (c == '>')
                {
                    Add("&gt;");
                }
                else if (c == '&')
                {
                    Add("&amp;");
                }
                else if (c == '"')
                {
                    Add("&quot;");
                }
                else
                {
                    Add(c);
                }
            }

        }

    }
}