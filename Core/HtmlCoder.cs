namespace Greatbone.Core
{
    public abstract class HtmlCoder<This> : Coder where This : HtmlCoder<This>
    {
        protected HtmlCoder(int initial) : base(initial)
        {
        }

        public override string ContentType => "text/html; charset=utf-8";


        public void _(string name, int v)
        {
        }

        public void _(string name, long v)
        {
        }

        public void _(string name, string v)
        {
        }
    }
}