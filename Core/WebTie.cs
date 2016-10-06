namespace Greatbone.Core
{
    public class WebTie
    {
        public string Key;

        public bool IsVar { get; internal set; }

        public WebSub Parent { get; internal set; }

        public WebService Service { get; internal set; }

    }
}