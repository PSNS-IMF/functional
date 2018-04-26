using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static List<T> Cons<T>() =>
            new List<T>();

        public static List<T> Cons<T>(params T[] items) =>
            new List<T>(items);

        public static Tuple<T1, T2> Tuple<T1, T2>(T1 t1, T2 t2) =>
            System.Tuple.Create(t1, t2);

        public static Task<T> AsTask<T>(this T t) =>
            Task.FromResult(t);
    }
}
