using System;
using System.Collections.Generic;
using System.Linq;

namespace Psns.Common.Functional
{
    /// <summary>
    /// Functional additions to <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// A matcher that differentiates an empty list from one with content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="empty"></param>
        /// <param name="more"></param>
        /// <returns></returns>
        public static R Match<T, R>(this IEnumerable<T> self, Func<R> empty, Func<T, IEnumerable<T>, R> more) =>
            self.Any()
                ? empty()
                : more(self.First(), self);

        /// <summary>
        /// Add a <typeparamref name="T"/> to the end of <paramref name="self"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> self, T tail)
        {
            foreach (var item in self)
                yield return item;

            yield return tail;
        }
    }
}
