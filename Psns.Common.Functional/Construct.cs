using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static IEnumerable<T> Empty<T>() =>
            Enumerable.Empty<T>();

        public static List<T> List<T>() =>
            new List<T>();

        public static IEnumerable<T> Cons<T>() =>
            new T[0];

        public static List<T> List<T>(params T[] items) =>
            new List<T>(items);

        public static IEnumerable<T> Cons<T>(params T[] items) =>
            items;

        /// <summary>
        /// Add a <typeparamref name="T"/> to the beginning of <paramref name="tail"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static IEnumerable<T> Cons<T>(this T head, IEnumerable<T> tail)
        {
            yield return head;

            foreach (var item in tail)
                yield return item;
        }

        public static Tuple<T1, T2> Tuple<T1, T2>(T1 t1, T2 t2) =>
            System.Tuple.Create(t1, t2);

        public static Task<T> AsTask<T>(this T t) =>
            Task.FromResult(t);
    }
}
