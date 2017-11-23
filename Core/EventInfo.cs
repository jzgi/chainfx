using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor of an event handler method.
    ///  </summary>
    public class EventInfo : IDoer
    {
        readonly Service service;

        readonly string key;

        readonly bool async;

        readonly bool subscript;

        readonly int limit;

        // void event(WebEventContext)
        readonly Action<EventContext> @do;

        // async Task event(WebEventContext)
        readonly Func<EventContext, Task> doasync;

        // void action(WebActionContext, string)
        readonly Action<EventContext, string> do2;

        // async Task action(WebActionContext, string)
        readonly Func<EventContext, string, Task> do2async;

        internal EventInfo(Service service, MethodInfo mi, bool async, bool subscript, int limit = 0)
        {
            this.service = service;
            this.key = mi.Name;
            this.async = async;
            this.subscript = subscript;
            this.limit = limit;

            if (async)
            {
                if (subscript)
                {
                    do2async = (Func<EventContext, string, Task>) mi.CreateDelegate(typeof(Func<EventContext, string, Task>), service);
                }
                else
                {
                    doasync = (Func<EventContext, Task>) mi.CreateDelegate(typeof(Func<EventContext, Task>), service);
                }
            }
            else
            {
                if (subscript)
                {
                    do2 = (Action<EventContext, string>) mi.CreateDelegate(typeof(Action<EventContext, string>), service);
                }
                else
                {
                    @do = (Action<EventContext>) mi.CreateDelegate(typeof(Action<EventContext>), service);
                }
            }
        }

        public Service Service => service;

        public string Key => key;

        public bool IsAsync => async;

        public bool HasSubscript => subscript;

        public int Limit => limit;

        // invoke the right event method
        internal void Do(EventContext ec, string arg)
        {
            if (HasSubscript)
            {
                do2(ec, arg);
            }
            else
            {
                @do(ec);
            }
        }

        // invoke the right event method
        internal async Task DoAsync(EventContext ec, string arg)
        {
            if (HasSubscript)
            {
                await do2async(ec, arg);
            }
            else
            {
                await doasync(ec);
            }
        }

        public override string ToString()
        {
            return Key;
        }
    }
}