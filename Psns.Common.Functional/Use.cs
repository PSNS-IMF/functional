using System;
using System.Threading.Tasks;

namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
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
    }
}
