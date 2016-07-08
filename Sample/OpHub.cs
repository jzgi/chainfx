using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// WWW
    public class OpHub : WebHub
    {
        public OpHub() : base(null)
        {
            AddSub<OpSpaceHub>("space", null); // /space/*

            AddSub<OpUserHub>("user", null); // /user/*
        }

        public override void Default(WebContext wc)
        {
            Console.WriteLine("start Action: ");

            string id = "";
        }

        public void Show(WebContext wc)
        {
        }

        protected override bool ResolveZone(string zoneId, WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}