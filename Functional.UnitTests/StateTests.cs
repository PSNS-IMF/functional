using NUnit.Framework;
using Psns.Common.Functional;
using System;
using static NUnit.StaticExpect.Expectations;
using static Psns.Common.Functional.Prelude;

namespace SystemExtensions.UnitTests.Functional.StateUnitTests
{
    [TestFixture]
    public class StateTests
    {
        public delegate BinaryState BinaryState();

        public static BinaryState On() =>
            new BinaryState(Off);

        public static BinaryState Off() =>
            new BinaryState(On);

        public static State<object, BinaryState> Next() =>
            ((object)null).State<object, BinaryState>(state => state ?? On());
    }
}