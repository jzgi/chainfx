using System.Collections.Generic;
using System.IO;

namespace Greatbone.Core
{
    /// <summary>The configurative settings and the establishment of creation context during initialization of the controller hierarchy.</summary>
    /// <remarks>It provides a strong semantic that enables the whole controller hierarchy to be established within execution of constructors, starting from the constructor of a service controller.</remarks>
    /// <example>
    /// public class FooService : WebService
    /// {
    ///         public FooService(WebServiceContext wsc) : base(wsc)
    ///         {
    ///                 AddSub&lt;BarSub&gt;();
    ///         }
    /// }
    /// </example>
    ///
    public class WebServiceContext
    {
        // SETTINGS
        //

        internal string key;

        // public socket address
        internal string @public;

        internal bool tls;

        // inner socket address
        internal string @internal;

        // event system socket addresses
        internal string[] foreign;

        internal bool debug;

        internal Dictionary<string, string> options;

        // CONTEXT
        //

        internal WebService Service { get; set; }

        readonly Stack<WebSub> chain = new Stack<WebSub>(8);

        internal WebSub Parent => (chain.Count == 0) ? null : chain.Peek();

        internal void Enter(WebSub current)
        {
            chain.Push(current);
        }

        internal WebSub Exit()
        {
            return chain.Pop();
        }

    }

}