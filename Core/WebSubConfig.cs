namespace Greatbone.Core
{
    public class WebSubConfig
    {
        // SETTINGS
        //

        public string Key;

        public bool Debug;

        public WebService Service { get; internal set; }

        public WebSub Parent { get; internal set; }

        public bool IsXed { get; internal set; }
    }
}