using System;

namespace ChainFX.Web
{
    /// <summary>
    /// To document an help information to the target work or action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class HelpAttribute : Attribute
    {
        readonly string[] texts;

        public HelpAttribute(params string[] texts)
        {
            this.texts = texts;
        }

        public bool IsDetail => true;

        public void Render(HtmlBuilder h)
        {
            if (texts != null)
            {
                foreach (var v in texts)
                {
                    h.P(v);
                }
            }
        }
    }
}