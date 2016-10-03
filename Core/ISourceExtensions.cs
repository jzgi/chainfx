using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class ISourceExtensions
    {
        public static T Load<T>(this ISource i) where T : IPersist, new()
        {
            T obj = new T();

            return obj;
        }

    }

}