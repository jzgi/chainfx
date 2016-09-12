using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public class MsgPoller : IMember
    {
        List<object> queue;

        public string Key
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}