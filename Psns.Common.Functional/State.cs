using System;
using static Psns.Common.Functional.Prelude;

namespace Psns.Common.Functional
{
    public delegate (TValue Value, TState State) State<TValue, TState>(TState state);

    public static partial class StateExtensions
    {
        public static State<TValue, TState> State<TValue, TState>(this TValue self) =>
            state => (self, state);

        public static State<TValue, TState> State<TValue, TState>(this TValue self, Func<TState, TState> newState) =>
            oldState => (self, newState(oldState));

        public static State<TValue, TState> Bind<TValue, TState>(
            this State<TValue, TState> self,
            Func<TValue, State<TValue, TState>> binder) =>
                self.Bind<TValue, TValue, TState>(binder);

        public static State<TResult, TState> Bind<TValue, TResult, TState>(
            this State<TValue, TState> self,
            Func<TValue, State<TResult, TState>> binder) => state => 
                Map(self(state), res => binder(res.Value)(res.State));

        public static State<TResult, TState> SelectMany<TSource, TState, TSelector, TResult>(
            this State<TSource, TState> self,
            Func<TSource, State<TSelector, TState>> selector,
            Func<TSource, TSelector, TResult> resultSelector) => state =>
                Map(self(state), sourceRes =>
                    Map(selector(sourceRes.Value)(sourceRes.State), selectorRes =>
                        resultSelector(sourceRes.Value, selectorRes.Value)
                            .State<TResult, TState>()(selectorRes.State)));

        public static State<TResult, TState> Select<TSource, TResult, TState>(
            this State<TSource, TState> self,
            Func<TSource, TResult> map) =>
                self.SelectMany(
                    value => map(value).State<TResult, TState>(),
                    (_, selectVal) => selectVal);
    }

    public static partial class Prelude
    {
        public static State<TValue, TState> State<TValue, TState>() =>
            state => (default(TValue), state);

        public static State<TValue, TState> State<TValue, TState>(TValue val) =>
            state => (val, state);

        public static State<TValue, TState> State<TValue, TState>(Func<TState, (TValue, TState)> func) =>
            state => func(state);

        public static State<UnitValue, TState> State<TState>(Action action) => state =>
        {
            action();
            return (Unit, state);
        };

        public static State<UnitValue, TState> State<TState>() =>
            Unit.State<UnitValue, TState>();

        public static State<TState, TState> GetState<TState>() => state =>
           (state, state);

        public static State<TState, TState> SetState<TState>(TState newState) => oldState =>
            (oldState, newState);

        public static State<UnitValue, TState> SetState<TState>(Func<TState, TState> newState) => oldState =>
            (Unit, newState(oldState));
    }
}
