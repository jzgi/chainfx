using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To document a work to the target action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class HelpAttribute : Attribute, IDocTag
    {
        readonly string[] texts;

        public HelpAttribute(params string[] texts)
        {
            this.texts = texts;
        }

        public void Render(HtmlBuilder h)
        {
            h.P_();

            if (texts != null)
            {
                h.T("<pre>");
                foreach (var v in texts)
                {
                    h.TT(v);
                }
                h.T("</pre>");
            }
            h._P();
        }
    }
}