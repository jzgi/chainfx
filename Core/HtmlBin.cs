namespace Greatbone.Core
{
    public abstract class HtmlBin<TThis> : Bin where TThis : HtmlBin<TThis>
    {
        protected HtmlBin(int initial) : base(initial)
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