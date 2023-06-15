using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChainFx.Nodal;

namespace ChainFx.Web
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

        // tool annotation for ui
        readonly ToolAttribute tool;

        readonly AuthenticateAttribute authenticate;

        readonly AuthorizeAttribute authorize;

        readonly TwinSpyAttribute twinSpy;

        // 4 possible forms of the action method
        //
        readonly Action<WebContext> @do;
        readonly Func<WebContext, Task> doAsync;
        readonly Action<WebContext, int> do2;
        readonly Func<WebContext, int, Task> do2Async;

        // help tags
        readonly HelpAttribute[] helps;

        // restful tags
        readonly RestAttribute[] rests;

        internal WebAction(WebWork work, MethodInfo mi, bool async, string subscript)
        {
            this.work = work;
            this.name = mi.Name == "default" ? string.Empty : mi.Name;
            this.relative = name == string.Empty ? "./" : name;
            this.async = async;
            this.subscript = subscript;

            ui = (UiAttribute)mi.GetCustomAttribute(typeof(UiAttribute), true);
            tool = (ToolAttribute)mi.GetCustomAttribute(typeof(ToolAttribute), true);
            authenticate = (AuthenticateAttribute)mi.GetCustomAttribute(typeof(AuthenticateAttribute), true);
            authorize = (AuthorizeAttribute)mi.GetCustomAttribute(typeof(AuthorizeAttribute), true);
            twinSpy = (TwinSpyAttribute)mi.GetCustomAttribute(typeof(TwinSpyAttribute), true);
            helps = (HelpAttribute[])mi.GetCustomAttributes(typeof(HelpAttribute), true);
            rests = (RestAttribute[])mi.GetCustomAttributes(typeof(RestAttribute), true);

            // create a doer delegate
            if (async)
            {
                if (subscript != null)
                {
                    do2Async = (Func<WebContext, int, Task>)mi.CreateDelegate(typeof(Func<WebContext, int, Task>), work);
                }
                else
                {
                    doAsync = (Func<WebContext, Task>)mi.CreateDelegate(typeof(Func<WebContext, Task>), work);
                }
            }
            else
            {
                if (subscript != null)
                {
                    do2 = (Action<WebContext, int>)mi.CreateDelegate(typeof(Action<WebContext, int>), work);
                }
                else
                {
                    @do = (Action<WebContext>)mi.CreateDelegate(typeof(Action<WebContext>), work);
                }
            }

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

        public string Icon => ui?.Icon;

        public string Label => ui?.Label;

        public string Tip => ui?.Tip;

        public string Dt => Label ?? Tip ?? name;

        public short Group => ui?.Group ?? 0;

        public ToolAttribute Tool => tool;

        public RestAttribute[] Rests => rests;

        public HelpAttribute[] HelpTags => helps;

        public AuthenticateAttribute Authenticate => authenticate;

        public AuthorizeAttribute Authorize => authorize;

        public TwinSpyAttribute TwinSpy => twinSpy;


        public bool DoAuthorize(WebContext wc, bool mock)
        {
            if (Authorize != null)
            {
                return wc.Principal != null && Authorize.Do(wc, mock);
            }

            return true;
        }

        internal void Do(WebContext wc, int subscpt)
        {
            if (do2 != null)
            {
                do2(wc, subscpt);
            }
            else
            {
                @do(wc);
            }
        }

        // invoke the right method
        internal async Task DoAsync(WebContext wc, int subscpt)
        {
            if (do2Async != null)
            {
                await do2Async(wc, subscpt);
            }
            else
            {
                await doAsync(wc);
            }
        }

        public override string ToString() => name;
    }
}