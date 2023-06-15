using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To document a description for the target work or action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class SummaryAttribute : HelpAttribute
    {
        readonly string text;

        public SummaryAttribute(string text)
        {
            this.text = text;
        }

        public override bool IsDetail => false;

        public string Text => text;

        public override void Render(HtmlBuilder h)
        {
            if (text != null)
            {
                h.TT(text);
            }
        }
    }
}