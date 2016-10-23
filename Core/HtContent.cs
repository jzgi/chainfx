namespace Greatbone.Core
{

    /// <summary>
    /// A fragment of dynamic generated HyperText ML content.
    /// </summary>
    ///
    public abstract class HtContent<R> : DynamicContent where R : HtContent<R>
    {
        public HtContent(int capacity) : base(capacity)
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

        public R T(string str)
        {
            Add(str);
            return (R)this;
        }

    }
}