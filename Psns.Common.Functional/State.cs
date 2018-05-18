using System;
using static Psns.Common.Functional.Prelude;

namespace Psns.Common.Functional
{
    public delegate Lazy<T, TState> State<T, TState>(TState state);

    public static partial class StateModule
    {
        public static State<TResult, TState> SelectMany<TSource, TState, TSelector, TResult>(
            this State<TSource, TState> source,
            Func<TSource, State<TSelector, TState>> selector,
            Func<TSource, TSelector, TResult> resultSelector) => state =>
                    Map(source(state), sourceRes =>
                        Map(selector(sourceRes.Value)(sourceRes.Metadata), selectorRes =>
                            new Lazy<TResult, TState>(() => 
                                resultSelector(sourceRes.Value, selectorRes.Value), selectorRes.Metadata)));
    }

    public static partial class Prelude
    {
        public static State<T, TState> State<T, TState>(this T value) => state =>
            new Lazy<T, TState>(() => value, state);

        public static State<UnitValue, TState> UnitState<TState>() =>
            Unit.State<UnitValue, TState>();

        public static State<TResult, TState> Select<TSource, TResult, TState>(
            this State<TSource, TState> source,
            Func<TSource, TResult> selector) =>
                source.SelectMany(
                    value => selector(value).State<TResult, TState>(), 
                    (_, selectVal) => selectVal);

        public static TSource Value<TSource, TState>(this State<TSource, TState> source, TState state) =>
            source(state).Value;

        public static TState State<T, TState>(this State<T, TState> source, TState state) =>
            source(state).Metadata;

        public static State<TState, TState> Get<TState>() => state =>
           new Lazy<TState, TState>(() => state, state);

        public static State<TState, TState> Set<TState>(Func<TState, TState> newState) => oldState =>
            new Lazy<TState, TState>(() => oldState, newState(oldState));
    }
}
