using System;

namespace Greatbone.Core
{
    internal struct MsgMessage : IContent
    {
        byte[] body;

        string topic;

        public byte[] Buffer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public long ETag
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DateTime LastModified
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Type => "msg/bin";

    }
}