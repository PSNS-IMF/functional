﻿using System;
using System.Threading.Tasks;
using static Psns.Common.Functional.Prelude;

namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static Try<T> Try<T>(Func<T> tryDel) => () => 
            tryDel();

        public static Try<UnitValue> Try(Action attempt) => () => 
            { attempt(); return Unit; };

        public static TryAsync<T> TryAsync<T>(Func<Task<T>> tryDel) => async () => 
            new TryResult<T>(await tryDel());

        public static TryAsync<UnitValue> TryAsync(Func<Task> tryDel) => async () =>
            { await tryDel(); return Unit; };

        public static Try<T> Fail<T>(Exception e) => () =>
            new TryResult<T>(e);

        public static TryAsync<T> FailAsync<T>(Exception e) => () =>
            new TryResult<T>(e).AsTask();
    }

    public delegate TryResult<T> Try<T>();

    public delegate Task<TryResult<T>> TryAsync<T>();

    public struct TryResult<T>
    {
        internal readonly T Value;
        internal readonly Exception Exception;

        internal TryResult(T value)
        {
            Value = value;
            Exception = null;
        }

        internal TryResult(Exception e)
        {
            Exception = e;
            Value = default(T);
        }

        internal bool IsFailure => Exception != null;

        public static implicit operator TryResult<T>(T value) =>
            new TryResult<T>(value);

        public static implicit operator TryResult<T>(Exception e) =>
            new TryResult<T>(e);

        public static implicit operator Either<Exception, T>(TryResult<T> res) =>
            res.IsFailure
                ? Left<Exception, T>(res.Exception)
                : Right<Exception, T>(res.Value);

        public static implicit operator TryResult<T>(Either<Exception, T> either) =>
            either.Match(
                right: t => new TryResult<T>(t), 
                left: e => new TryResult<T>(e));
    }

    public static class TryExtensions
    {
        public static Try<R> Bind<T, R>(this Try<T> self, Func<T, Try<R>> binder) => () =>
            Map(self.Try(), res => res.IsFailure
                ? new TryResult<R>(res.Exception)
                : binder(res.Value).Try());

        public static Try<T> Bind<T>(this Try<T> self, Try<UnitValue> binder) => () =>
            Map(self.Try(), res => res.IsFailure
                ? res.Exception
                : Map(binder(), _ => res));

        public static Try<R> Bind<T, R>(this Try<T> self, Func<T, R> binder) => () =>
            Map(self.Try(), res => res.IsFailure
                ? res.Exception
                : Prelude.Try(() => binder(res.Value)).Try());

        public static Try<R> Bind<T, R>(this Try<T> self, Func<T, TryResult<R>> binder) => () =>
            Map(self.Try(), res => res.IsFailure
                ? res.Exception
                : binder(res.Value));

        /// <summary>
        /// Executes binder function even if self fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="binder"></param>
        /// <returns>
        ///     If binder succeeds, self.
        ///     Else If binder fails, 
        ///         if self fails, self's exception; 
        ///         otherwise, binder's exception.</returns>
        public static Try<R> Regardless<T, R>(this Try<T> self, Try<R> binder) => () =>
            Map(self.Try(), res =>
                Map(binder(), r => r.IsFailure
                    ? new TryResult<R>(r.Exception)
                    : res.IsFailure ? new TryResult<R>(res.Exception) : r));

        public static TryAsync<R> Bind<T, R>(this Try<T> self, Func<T, TryAsync<R>> binder) => async () =>
            await Map(self.Try(), async res => res.IsFailure
                ? res.Exception
                : await binder(res.Value).TryAsync());

        public static Try<T> Regardless<T>(this Try<T> self, Try<UnitValue> binder) => () =>
            Map(self.Try(), res =>
                Map(binder(), r => r.IsFailure 
                    ? new TryResult<T>(r.Exception)
                    : res));

        public static Try<R> BindIf<T, R>(
            this Try<T> self,
            Func<T, Try<R>> binder,
            Func<R, bool> predicate,
            Func<T, R> falseMap) => () =>
                Map(self.Try(), selfRes =>
                    selfRes.IsFailure
                        ? new TryResult<R>(selfRes.Exception)
                        : Map(binder(selfRes.Value).Try(), bindRes =>
                            bindRes.IsFailure
                                ? new TryResult<R>(bindRes.Exception)
                                : predicate(bindRes.Value)
                                    ? bindRes
                                    : falseMap(selfRes.Value)));

        public static TryAsync<R> Bind<T, R>(this TryAsync<T> self, Func<T, TryAsync<R>> binder) => async () =>
            await Map(await self.TryAsync(), async res => res.IsFailure
                ? res.Exception
                : await binder(res.Value).TryAsync());

        public static TryAsync<R> Bind<T, R>(this TryAsync<T> self, Func<T, Task<R>> binder) => async () =>
            await Map(await self.TryAsync(), async res => res.IsFailure
                ? res.Exception
                : await Prelude.TryAsync(() => binder(res.Value)).TryAsync());

        public static TryAsync<T> Bind<T>(this TryAsync<T> self, TryAsync<UnitValue> binder) => async () =>
            await Map(await self.TryAsync(), async res => res.IsFailure
                ? res.Exception
                : Map(await binder(), _ => res));

        public static TryAsync<R> Bind<T, R>(this TryAsync<T> self, Func<T, Task<Either<Exception, R>>> binder) => async () =>
            await Map(await self.TryAsync(), async res =>
                res.IsFailure
                    ? res.Exception
                    : await binder(res.Value));

        public static TryAsync<R> Regardless<T, R>(this TryAsync<T> self, TryAsync<R> binder) => async () =>
            await Map(await self.TryAsync(), async res =>
                Map(await binder.TryAsync(), r => r.IsFailure
                    ? new TryResult<R>(r.Exception)
                    : res.IsFailure ? new TryResult<R>(res.Exception) : r));

        public static TryAsync<T> Regardless<T>(this TryAsync<T> self, TryAsync<UnitValue> binder) => async () =>
            await Map(await self.TryAsync(), async res => 
                Map(await binder.TryAsync(), r => r.IsFailure 
                    ? new TryResult<T>(r.Exception) 
                    : res));

        public static TryAsync<R> BindIf<T, R>(
            this TryAsync<T> self,
            Func<T, TryAsync<R>> binder,
            Func<R, bool> predicate,
            Func<T, R> falseMap) => async () =>
                await Map(await self.TryAsync(), async selfRes =>
                    selfRes.IsFailure
                        ? new TryResult<R>(selfRes.Exception)
                        : Map(await binder(selfRes.Value).TryAsync(), bindRes =>
                            bindRes.IsFailure
                                ? new TryResult<R>(bindRes.Exception)
                                : predicate(bindRes.Value)
                                    ? bindRes
                                    : falseMap(selfRes.Value)));

        public static TryResult<T> Try<T>(this Try<T> self)
        {
            try
            {
                return self();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static async Task<TryResult<T>> TryAsync<T>(this TryAsync<T> self)
        {
            try
            {
                return await self();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static R Match<T, R>(this Try<T> self, Func<T, R> success, Func<Exception, R> fail) =>
            Map(self.Try(), res => res.IsFailure
                ? fail(res.Exception)
                : success(res.Value));

        public static UnitValue Match<T>(this Try<T> self, Action<T> success, Action<Exception> fail) =>
            self.Match(t => { success(t); return Unit; }, e => { fail(e); return Unit; });

        public static S Match<T, R, S>(this Try<T> self, Func<T, R> success, Func<Exception, R> fail, Func<R, S> onEither) =>
            Map(self.Match(success, fail), r => onEither(r));

        public static async Task<R> Match<T, R>(this TryAsync<T> self, Func<T, R> success, Func<Exception, R> fail) =>
            Map(await self.TryAsync(), res => res.IsFailure
                ? fail(res.Exception)
                : success(res.Value));

        public static async Task<S> Match<T, R, S>(this TryAsync<T> self, Func<T, R> success, Func<Exception, R> fail, Func<R, S> onEither) =>
            Map(await self.Match(success, fail), r => onEither(r));

        public static Task<Either<Exception, T>> ToEither<T>(this TryAsync<T> self) =>
            self.Match(
                val => Right<Exception, T>(val),
                e => e);
    }
}