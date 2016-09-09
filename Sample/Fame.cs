using System;
using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class Fame : ISerial
    {
        internal char[] id;
        internal string name;
        internal string quote;
        internal bool sex;
        internal byte[] icon;
        internal DateTime birthday;
        internal string qq;
        internal string wechat;
        internal string email;
        internal string province;
        internal string city;
        internal short rank;
        internal short height;
        internal short weight;
        internal short bust;
        internal short waist;
        internal short hip;
        internal short cup;
        internal short styles;
        internal short skills;
        internal short remark;
        internal List<Item> sites;
        internal List<Item> friends;
        internal List<Item> awards;

        public void ReadFrom(ISerialReader r)
        {
            throw new System.NotImplementedException();
        }

        public void WriteTo(ISerialWriter w)
        {
            throw new System.NotImplementedException();
        }
    }

    public struct Item
    {
        internal char[] uid;

        internal string url;

        internal string desc;
    }
}