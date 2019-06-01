using System;
using System.Collections;
using System.Collections.Generic;

namespace Greatbone.Db
{
    public class DbSet : IEnumerable<DbRow>
    {
        public IEnumerator<DbRow> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DbRow
    {
    }
}