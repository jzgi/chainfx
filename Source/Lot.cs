using System;
using System.Collections.Generic;

namespace SkyChain
{
    public class Lot<D, V> : Map<D, List<V>> where D : IComparable<D>
    {
        readonly Func<V, D> func;

        public Lot(Func<V, D> func) : base(16)
        {
            this.func = func;
        }

        public void Absorb<K>(Map<K, V> src)
        {
            for (int i = 0; i < src.Count; i++)
            {
                var v = src.ValueAt(i);
                var discr = func(v);
                var lst = this[discr];
                if (lst == null)
                {
                    lst = new List<V>(16);
                    Add(discr, lst);
                }
                lst.Add(v);
            }
        }
    }
}