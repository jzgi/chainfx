namespace Greatbone.Core
{
    public class WebSubConfig
    {
        // SETTINGS
        //

        internal string key;

        internal bool debug;

        public WebService Service { get; internal set; }

        public WebSub Parent { get; internal set; }

        public bool IsXed { get; internal set; }
    }
}