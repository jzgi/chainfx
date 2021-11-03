﻿using System;
using System.Reflection;
using System.Threading.Tasks;
 using SkyChain;

 namespace SkyChain.Chain
{
    public class ChainAction : IKeyable<string>
    {
        // the declaring logic
        readonly ChainDrive bean;

        // coding name
        readonly string name;

        // description
        readonly string tip;

        readonly bool async;

        readonly VerAttribute ver;

        // forms of the operation method
        //
        readonly Func<ChainContext, bool> @do;
        readonly Func<ChainContext, Task<bool>> doAsync;

        internal ChainAction(ChainDrive drive, MethodInfo mi, bool async)
        {
            this.bean = drive;
            this.name =  mi.Name;
            this.tip = name == string.Empty ? "./" : name;
            this.async = async;

            ver = (VerAttribute) mi.GetCustomAttribute(typeof(VerAttribute), true);

            // create a doer delegate
            if (async)
            {
                doAsync = (Func<ChainContext, Task<bool>>) mi.CreateDelegate(typeof(Func<ChainContext, Task<bool>>), drive);
            }
            else
            {
                @do = (Func<ChainContext, bool>) mi.CreateDelegate(typeof(Func<ChainContext, bool>), drive);
            }
        }

        public ChainDrive Drive => bean;

        public Func<ChainContext, bool> Do => @do;

        public Func<ChainContext, Task<bool>> DoAsync => doAsync;

        public string Name => name;

        public string Key => name;

        public string Tip => tip;

        public bool IsAsync => async;

        public VerAttribute Ver => ver;
    }
}