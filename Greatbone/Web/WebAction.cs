﻿using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Greatbone.Web
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

        // state check annotation
        readonly StateAttribute state;


        // 4 possible forms of the action method
        //
        readonly Action<WebContext> @do;
        readonly Func<WebContext, Task> doAsync;
        readonly Action<WebContext, string> do2;
        readonly Func<WebContext, string, Task> do2Async;

        // commenting tags
        readonly TagAttribute[] tags;

        internal WebAction(WebWork work, MethodInfo mi, bool async, string subscript)
        {
            this.work = work;
            this.name = mi.Name;
            this.relative = name == string.Empty ? "./?inner=true" : name;
            this.async = async;
            this.subscript = subscript;

            this.tool = (ToolAttribute) mi.GetCustomAttribute(typeof(ToolAttribute), true);
            this.state = (StateAttribute) mi.GetCustomAttribute(typeof(StateAttribute), true);

            // create a doer delegate
            if (async)
            {
                if (subscript != null)
                {
                    do2Async = (Func<WebContext, string, Task>) mi.CreateDelegate(typeof(Func<WebContext, string, Task>), work);
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
                    do2 = (Action<WebContext, string>) mi.CreateDelegate(typeof(Action<WebContext, string>), work);
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

        public string Label => ui?.Label;

        public string Tip => ui?.Tip;

        public byte Sort => ui?.Sort ?? 0;

        public AuthenticateAttribute Authenticate => authenticate;

        public AuthorizeAttribute Authorize => authorize;

        public ToolAttribute Tool => tool;

        public StateAttribute State => state;

        public TagAttribute[] Tags => tags;

        public bool DoAuthorize(WebContext wc)
        {
            if (Authorize != null)
            {
                // check if trusted peer
                if (wc.CallerSign != null && wc.CallerSign == Framework.Signature)
                {
                    return true; // trusted without further check
                }

                return Authorize.Do(wc);
            }

            return true;
        }

        public bool CheckState(WebContext wc, object[] stack, int level)
        {
            return state == null || stack == null || state.Check(wc, stack, level);
        }

        internal void Do(WebContext wc, string subscript)
        {
            if (subscript == null)
            {
                do2(wc, subscript);
            }
            else
            {
                @do(wc);
            }
        }

        // invoke the right method
        internal async Task DoAsync(WebContext wc, string subscript)
        {
            if (subscript == null)
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