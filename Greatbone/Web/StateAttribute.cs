using System;

namespace Greatbone.Web
{
    /// <summary>
    /// A constraint check to determine the availability of a procedure.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class StateAttribute : Attribute
    {
        /// <summary>
        /// Determines the availability of the action method.
        /// </summary>
        /// <param name="wc">The web context</param>
        /// <param name="model">The current model object</param>
        /// <returns>true if the procedure is available</returns>
        public abstract bool Check(WebContext wc, object model);
    }
}