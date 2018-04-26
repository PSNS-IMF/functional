using System;

namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static Func<R> Compose<T, R>(this Func<T, R> self, Func<T> func) => () =>
            self(func());

        public static Func<T1, R> Compose<T1, T2, R>(this Func<T2, R> self, Func<T1, T2> func) =>
            t => self(func(t));

        public static Func<R> Par<T1, R>(this Func<T1, R> self, T1 t1) => () =>
            self(t1);

        public static Func<T2, R> Par<T1, T2, R>(this Func<T1, T2, R> self, T1 t1) => t2 =>
            self(t1, t2);

        public static Func<R> Par<T1, T2, R>(this Func<T1, T2, R> self, T1 t1, T2 t2) => () =>
            self(t1, t2);

        public static Func<T3, R> Par<T1, T2, T3, R>(this Func<T1, T2, T3, R> self, T1 t1, T2 t2) => t3 =>
            self(t1, t2, t3);

        public static Func<T3, T4, R> Par<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> self, T1 t1, T2 t2) => (T3 t3, T4 t4) =>
            self(t1, t2, t3, t4);

        public static Func<T4, R> Par<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> self, T1 t1, T2 t2, T3 t3) => t4 =>
            self(t1, t2, t3, t4);

        public static Func<R> Par<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> self, T1 t1, T2 t2, T3 t3, T4 t4) => () =>
            self(t1, t2, t3, t4);

        public static Func<T5, R> Par<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> self, T1 t1, T2 t2, T3 t3, T4 t4) => t5 =>
            self(t1, t2, t3, t4, t5);

        public static Func<T4, T5, R> Par<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> self, T1 t1, T2 t2, T3 t3) => (t4, t5) =>
            self(t1, t2, t3, t4, t5);

        public static Func<T6, R> Par<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> self, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => t6 =>
            self(t1, t2, t3, t4, t5, t6);

        public static Func<T5, T6, R> Par<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> self, T1 t1, T2 t2, T3 t3, T4 t4) => (t5, t6) =>
            self(t1, t2, t3, t4, t5, t6);
    }
}
