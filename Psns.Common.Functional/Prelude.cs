using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static R Map<T, R>(T value, Func<T, R> map) =>
            map(value);

        public static R Map<T, R>(T value, Action<T> with, R res = default(R))
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
            await Map(cancelToken ?? Task.Factory.CancellationToken, async token =>
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
                        none: () => default(R));
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

        public static R Use<T, R>(T disposable, Func<T, R> user) where T : IDisposable =>
            Use(() => disposable, user);

        public static R Use<T, R>(Func<T> factory, Func<T, R> user) where T : IDisposable
        {
            var disposable = factory();

            try
            {
                var result = user(disposable);
                return result;
            }
            finally
            {
                disposable.Dispose();
            }
        }

        public static async Task<R> UseAsync<T, R>(T disposable, Func<T, Task<R>> user) where T : IDisposable =>
            await UseAsync(() => disposable, user);

        public static async Task<R> UseAsync<T, R>(Func<T> factory, Func<T, Task<R>> user) where T : IDisposable
        {
            var disposable = factory();

            try
            {
                return await user(disposable);
            }
            finally
            {
                disposable.Dispose();
            }
        }

        public static Try<R> TryUse<T, R>(Func<T> factory, Func<T, R> user) where T : IDisposable =>
            Try(factory).Bind(val => Use(val, user));

        public static Try<R> TryUse<T, R>(Func<T> factory, Func<T, Try<R>> user) where T : IDisposable =>
            Try(factory).Bind(val => Use(val, _ => user(val).Try()));

        public static TryAsync<R> TryUse<T, R>(Func<T> factory, Func<T, Task<R>> user) where T : IDisposable =>
            Try(factory)
                .Bind(val => TryAsync(() => UseAsync(val, user)));

        public static TryAsync<R> TryUse<T, R>(Func<T> factory, Func<T, TryAsync<R>> user) where T : IDisposable => async () =>
            await Try(factory)
                .Bind(async val => await UseAsync(val, v => user(v).TryAsync()))
                .Match(task => task, ex => new TryResult<R>(ex).AsTask());

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
    }
}
