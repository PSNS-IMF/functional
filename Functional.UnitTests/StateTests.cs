﻿using NUnit.Framework;
using Psns.Common.Functional;
using System.Collections.Generic;
using System.Linq;
using static NUnit.StaticExpect.Expectations;
using static Psns.Common.Functional.Prelude;

namespace SystemExtensions.UnitTests.Functional.StateUnitTests
{
    [TestFixture]
    public class StateTests
    {
        public static State<T, IEnumerable<T>> Pop<T>() =>
            source => (source.First(), source.Skip(1));

        public static State<T, IEnumerable<T>> Push<T>(T val) =>
            source => (val, source.Append(val));

        [Test]
        public void StateShouldBeUpdatable()
        {
            var query =
                from v1 in Push(1)
                from v2 in Push(3)
                from v3 in Pop<int>()
                from v4 in Push(2)
                select v4;

            var query2 =
                from v1 in query
                from v2 in Push(5)
                select v2;

            var result = query2(Empty<int>());

            CollectionAssert.AreEqual(Cons(3, 2, 5), result.State);
            Expect(result.Value, EqualTo(5));
        }

        [Test]
        public void StateShouldBeChainable()
        {
            var initial =
                State<int, IEnumerable<int>>()
                    .Bind(val => state => (val + 1, state.Append(1)))
                    .Bind(val => state => (val + 2, state.Append(2)));

            var result = initial(Empty<int>());

            CollectionAssert.AreEqual(Cons(1, 2), result.State);
            Expect(result.Value, EqualTo(3));
        }
    }

    [TestFixture]
    public class StopLightSimulation
    {
        delegate (string, ColorState) ColorState();

        static ColorState RedState = () =>
            (nameof(RedState), () => GreenState());

        static ColorState YellowState = () =>
            (nameof(YellowState), () => RedState());

        static ColorState GreenState = () =>
            (nameof(GreenState), () => YellowState());

        static State<string, ColorState> machine() =>
            State<string, ColorState>(state =>
                map(
                    state(), 
                    s => (s.Item1, s.Item2)));

        [Test] 
        public void ItShouldProgressStatesCorrectly()
        {
            var query =
                from fst in machine()
                from sec in machine()
                select sec;

            var from = fun((ColorState starting) =>
                query(starting).State().Item1);

            Expect(
                (from(GreenState), from(YellowState), from(RedState)), 
                EqualTo((nameof(RedState), nameof(GreenState), nameof(YellowState))));
        }
    }
}