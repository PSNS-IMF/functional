using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static bool IsNull<T>(T value) =>
            value == null
                || (Nullable.GetUnderlyingType(typeof(T)) != null && value.Equals(default(T)));

        public static bool IsDefault<T>(T value) =>
            EqualityComparer<T>.Default.Equals(value, default);

        public static R map<T, R>(T value, Func<T, R> map) =>
            map(value);

        public static R map<T, R>(T value, Action<T> with, R res = default)
        {
            if (value != null)
                with(value);

            return res;
        }

        public static T Tap<T>(this T value, params Action<T>[] actions)
        {
            foreach(var action in actions)
            {
                action(value);
            }

            return value;
        }

        public static async Task<T> TapAsync<T>(this T value, CancellationToken? cancelToken = null, params Action<T>[] actions) =>
            await map(cancelToken ?? Task.Factory.CancellationToken, async token =>
                await Task.Run(async () =>
                {
                    foreach (var action in actions)
                    {
                        await Task.Factory.StartNew(() =>
                            {
                                action(value);

                                token.ThrowIfCancellationRequested();
                            },
                            token,
                            TaskCreationOptions.AttachedToParent,
                            TaskScheduler.Current);
                    }

                    token.ThrowIfCancellationRequested();

                    return value;
                }, token));

        public static R Match<T, R>(T self, params Func<T, Maybe<R>>[] matchers)
        {
            foreach(var matcher in matchers)
            {
                var possible = matcher(self);
                if (possible.IsSome)
                {
                    return possible.Match(
                        some: r => r, 
                        none: () => default);
                }
            }

            throw new InvalidOperationException("No match was found");
        }

        public static Func<T, Maybe<R>> AsEqual<T, R>(T value, Func<T, R> map) => t =>
            EqualityComparer<T>.Default.Equals(value, t)
                ? Some(map(value))
                : Maybe<R>.None;

        public static Func<T, Maybe<R>> NotEqual<T, R>(T value, Func<T, R> map) => t =>
            !EqualityComparer<T>.Default.Equals(value, t)
                ? Some(map(value))
                : Maybe<R>.None;

        /// <summary>
        /// Checks for null value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="source">An optional string to be used as the Exception Message</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <returns>The <paramref name="value"/> if not null</returns>
        public static T AssertValue<T>(this T value, string source = null)
        {
            if (IsNull(value))
            {
                throw new ArgumentNullException(source ?? nameof(T));
            }
            
            return value;
        }

        public static Action<T> act<T>(Action<T> self) => self;

        public static Func<R> fun<R>(Func<R> f) => f;

        public static Func<T1, R> fun<T1, R>(Func<T1, R> f) => f;

        public static Func<T1, T2, R> fun<T1, T2, R>(Func<T1, T2, R> f) => f;

        /// <summary>
        /// Throws an <see cref="Exception"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message to be used for the <see cref="Exception"/></param>
        /// <returns></returns>
        public static T failwith<T>(string message)
        {
            throw new Exception(message);
        }

        /// <summary>
        /// Throws a given <see cref="Exception"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static T raise<T>(Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// Create a function that returns a <see cref="Task"/> that does nothing.
        /// </summary>
        public static Func<Task> UnitTask => () =>
            Task.Delay(0);

        public static int random(int max) =>
            new Random().Next(max);

        public static IEnumerable<int> range(int start, int count) =>
            Enumerable.Range(start, count);
    }
}
