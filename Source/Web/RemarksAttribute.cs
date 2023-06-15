using System;

namespace ChainFx.Web
{
    /// <summary>
    /// To document a supplemental or detail information to the target work or action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class RemarksAttribute : HelpAttribute
    {
        readonly string[] texts;

        public RemarksAttribute(params string[] texts)
        {
            this.texts = texts;
        }

        public override bool IsDetail => true;

        public override void Render(HtmlBuilder h)
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