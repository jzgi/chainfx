using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public struct PostImg
    {
        internal int postid;

        internal string name;

        internal ArraySegment<byte> bytes;


        public static void Load(DbContext dc, int postid)
        {

        }
    }
}