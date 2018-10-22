using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Greatbone
{
    public class Roll<K, V, N> : IKeyable<K> where N : IEquatable<N>, IComparable<N>
    {
        readonly int capacity;

        readonly K key;

        V[] array;

        int count;

        N sum;

        public Roll(K key, int capacity = 16)
        {
            this.capacity = capacity;
            this.key = key;
            array = null;
            count = 0;
        }

        public void Add(V v, N num)
        {
            // ensure capacity
            if (array == null)
            {
                array = new V[capacity];
            }
            else
            {
                int len = array.Length;
                if (count >= len)
                {
                    V[] alloc = new V[len * 4];
                    Array.Copy(array, 0, alloc, 0, len);
                    array = alloc;
                }
            }
            array[count++] = v;
            // sum up the number
            if (num.Equals(default))
            {
                sum = AddOp(sum, num);
            }
        }

        public K Key => key;

        public int Count => count;

        public V this[int idx] => array[idx];


        // ReSharper disable once StaticMemberInGenericType
        static Delegate addOp;

        static T AddOp<T>(T a, T b)
        {
            // declare the parameters and the expression
            var pa = Parameter(typeof(T), "a");
            var pb = Parameter(typeof(T), "b");
            var expr = Expression.Add(pa, pb);
            // compile the expression
            if (addOp == null)
            {
                addOp = Lambda<Func<T, T, T>>(expr, pa, pb).Compile();
            }
            return ((Func<T, T, T>) addOp)(a, b);
        }
    }

    public static class RollUtility
    {
        public static Roll<K, V, N>[] RollUp<K, V, N>(this V[] array, Func<V, (K, N)> keyer)
            where N : IEquatable<N>, IComparable<N>
        {
            if (array == null) return null;

            List<Roll<K, V, N>> list = new List<Roll<K, V, N>>();
            Roll<K, V, N> roll = null;
            for (int i = 0; i < array.Length; i++)
            {
                var v = array[i];
                var ( key, num) = keyer(v);
                if (roll == null)
                {
                    roll = new Roll<K, V, N>(key);
                    list.Add(roll);
                    roll.Add(v, num);
                }
                else if (key.Equals(roll.Key))
                {
                    roll.Add(v, num);
                }
                else // create a new sort
                {
                    roll = new Roll<K, V, N>(key);
                    list.Add(roll);
                    roll.Add(v, num);
                }
            }
            return list.ToArray();
        }
    }
}