using System;
using System.Text;

namespace Greatbone
{
    /// <summary>
    /// Tag for API documentation comments. 
    /// </summary>
    public abstract class TagAttribute : Attribute
    {
        internal abstract void Print(HtmlContent h);

        internal string Preprocess(string v)
        {
            StringBuilder sb = new StringBuilder(v.Length);
            int line = 0;
            for (int i = 0; i < v.Length; i++)
            {
                char c = v[i];
                if (c == '\n')
                {
                    line++;
                }
            }
            return sb.ToString();
        }
    }
}