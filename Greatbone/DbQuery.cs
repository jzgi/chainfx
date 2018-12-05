using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Greatbone
{
    public class DbQuery : IQueryable
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }
    }
}