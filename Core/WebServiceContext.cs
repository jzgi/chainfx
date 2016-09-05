﻿namespace Greatbone.Core
{
    public class WebServiceContext
    {
        // SETTINGS
        //

        internal string key;

        internal bool debug;

        public WebService Service { get; internal set; }

        public WebSub Parent { get; internal set; }

        public bool IsX { get; internal set; }
    }
}