using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Skyiah.Web
{
    /// <summary>
    /// The descriptor for an action method. 
    /// </summary>
    public sealed class WebAction : IKeyable<string>
    {
        // the governing work
        readonly WebWork work;

        readonly string name;

        // relative path
        readonly string relative;

        readonly bool async;

        // the name of subscript parameter (the second parameter), may be null
        readonly string subscript;

        // the action pathing 
        readonly string pathing;

        readonly UiAttribute ui;

        readonly AuthenticateAttribute authenticate;

        readonly AuthorizeAttribute authorize;

        // tool annotation for ui
        readonly ToolAttribute tool;


        // 4 possible forms of the action method
        //
        readonly Action<WebContext> @do;
        readonly Func<WebContext, Task> doAsync;
        readonly Action<WebContext, int> do2;
        readonly Func<WebContext, int, Task> do2Async;

        // commenting tags
        readonly TagAttribute[] tags;

        internal WebAction(WebWork work, MethodInfo mi, bool async, string subscript)
        {
            this.work = work;
            this.name = mi.Name == "default" ? string.Empty : mi.Name;
            this.relative = name == string.Empty ? "./?inner=true" : name;
            this.async = async;
            this.subscript = subscript;

            ui = (UiAttribute) mi.GetCustomAttribute(typeof(UiAttribute), true);
            authenticate = (AuthenticateAttribute) mi.GetCustomAttribute(typeof(AuthenticateAttribute), true);
            authorize = (AuthorizeAttribute) mi.GetCustomAttribute(typeof(AuthorizeAttribute), true);
            tool = (ToolAttribute) mi.GetCustomAttribute(typeof(ToolAttribute), true);

            // create a doer delegate
            if (async)
            {
                if (subscript != null)
                {
                    do2Async = (Func<WebContext, int, Task>) mi.CreateDelegate(typeof(Func<WebContext, int, Task>), work);
                }
                else
                {
                    doAsync = (Func<WebContext, Task>) mi.CreateDelegate(typeof(Func<WebContext, Task>), work);
                }
            }
            else
            {
                if (subscript != null)
                {
                    do2 = (Action<WebContext, int>) mi.CreateDelegate(typeof(Action<WebContext, int>), work);
                }
                else
                {
                    @do = (Action<WebContext>) mi.CreateDelegate(typeof(Action<WebContext>), work);
                }
            }

            // comments
            var vlst = new ValueList<TagAttribute>(8);
            foreach (var m in mi.GetCustomAttributes())
            {
                if (m is TagAttribute c)
                {
                    vlst.Add(c);
                }
            }

            tags = vlst.ToArray();

            // resolve the action pathing
            var sb = new StringBuilder(work.Pathing);
            sb.Append(name);
            if (subscript != null)
            {
                sb.Append('-').Append('<').Append(subscript).Append('>');
            }

            pathing = sb.ToString();
        }

        public WebWork Work => work;

        public string Name => name;

        public string Key => name;

        public string Relative => relative;

        public bool IsAsync => async;

        public string Subscript => subscript;

        public string Pathing => pathing;

        public UiAttribute Ui => ui;

        public string Label => ui?.Label ?? Name;

        public string Tip => ui?.Tip;

        public byte Group => ui?.Group ?? 0;

        public AuthenticateAttribute Authenticate => authenticate;

        public AuthorizeAttribute Authorize => authorize;

        public ToolAttribute Tool => tool;

        public TagAttribute[] Tags => tags;

        public bool DoAuthorize(WebContext wc)
        {
            if (Authorize != null)
            {
                // check if trusted peer
                // todo check ip instead
                // if (wc.CallerSign != null && wc.CallerSign == Framework.sign)
                // {
                //     return true; // trusted without further check
                // }

                return Authorize.Do(wc);
            }

            return true;
        }

        internal void Do(WebContext wc, int subscript)
        {
            if (do2 != null)
            {
                do2(wc, subscript);
            }
            else
            {
                @do(wc);
            }
        }

        // invoke the right method
        internal async Task DoAsync(WebContext wc, int subscript)
        {
            if (do2Async != null)
            {
                await do2Async(wc, subscript);
            }
            else
            {
                await doAsync(wc);
            }
        }
    }
}