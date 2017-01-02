using System;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// The descriptor for an action method.
    ///
    public class WebAction : WebControl, IRollable
    {
        readonly WebFolder folder;

        readonly string name;

        readonly Action<WebActionContext> doer;

        readonly UiAttribute ui;

        internal WebAction(WebFolder folder, MethodInfo mi) : base(mi)
        {
            this.folder = folder;
            name = mi.Name;
            doer = (Action<WebActionContext>)mi.CreateDelegate(typeof(Action<WebActionContext>), folder);

            // initialize ui
            var uis = (UiAttribute[])mi.GetCustomAttributes(typeof(UiAttribute), false);
            if (uis.Length > 0) ui = uis[0];
        }

        public WebFolder Folder => folder;

        public string Name => name;

        public int Form => ui == null ? 0 : ui.Form;

        public string Label => ui?.Label ?? name;

        public string Icon => ui?.Icon;

        public int Dialog => ui == null ? 0 : ui.Dialog;

        public override WebService Service => folder.Service;

        public UiAttribute Ui => ui;

        internal void Do(WebActionContext ac)
        {
            ac.Action = this;
            if (Check(ac)) // authorize check
            {
                // pre-
                BeforeDo(ac);
                // invoke the action method
                doer(ac);
                // post-
                AfterDo(ac);
            }
            ac.Action = null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}