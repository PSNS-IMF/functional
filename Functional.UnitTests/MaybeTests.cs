using NUnit.Framework;
using Psns.Common.Functional;
using static NUnit.StaticExpect.Expectations;
using static Psns.Common.Functional.Prelude;

namespace SystemExtensions.UnitTests.Functional
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void ConstructingExplicitSome_WithNull_Throws() =>
            Expect(() => Some<string>(null), Throws.ArgumentNullException);

        [Test]
        public void ConstructingExplicitSome_WithDefaultValueType_Throws() =>
            Expect(() => Some(default(TestValueType?)), Throws.ArgumentNullException);

        static Maybe<string>[] someFactory =
        {
            Some("string"),
            Possible("string"),
            "string"
        };

        [TestCaseSource("someFactory")]
        public void ConstructingSome_ReturnsMaybe(Maybe<string> maybe) =>
            Expect(Resolve(maybe), EqualTo("some"));

        static Maybe<object>[] noneCases =
        {
            Maybe<object>.None,
            null
        };

        [TestCaseSource("noneCases")]
        public void Constructing_ReferenceTypeNone_ReturnsNone(Maybe<object> maybe) =>
            Expect(Resolve(maybe), EqualTo("none"));

        [Test]
        public void Constructing_ValueTypeNone_ReturnsNone() =>
            Expect(Resolve(Possible(default(TestValueType?))), EqualTo("none"));

        [Test]
        public void Binding_Somes_NewValueCreated() =>
            Expect(
                Some("start")
                    .Bind(start => Some($"{start} end"))
                    .Match(s => s, () => "none"),
                EqualTo("start end"));

        static Maybe<string>[] combinations =
        {
            Some("start").Bind(s => Maybe<string>.None),
            Maybe<string>.None.Bind(s => Some("end"))
        };

        [TestCaseSource("combinations")]
        public void Binding_WithNone_NoneReturned(Maybe<string> maybe) =>
            Expect(Resolve(maybe), EqualTo("none"));

        struct TestValueType { }

        string Resolve<T>(Maybe<T> m) =>
            m.Match(_ => "some", () => "none");
    }
}