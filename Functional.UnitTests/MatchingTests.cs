using NUnit.Framework;
using static NUnit.StaticExpect.Expectations;
using static Psns.Common.Functional.Prelude;

namespace SystemExtensions.UnitTests.Functional
{
    [TestFixture]
    public class MatchingTests
    {
        [Test]
        public void Should_Match_Bool() =>
            Expect(
                Match(
                    false, 
                    AsEqual(true, _ => true), 
                    _ => _),
                False);

        [Test]
        public void Should_MatchFirst_Bool() =>
            Expect(
                Match(
                    true,
                    AsEqual(true, _ => true)),
                True);

        [Test]
        public void NoMatch_Should_Throw() =>
            Expect(() =>
                Match(
                    false,
                    AsEqual(true, _ => true)),
                Throws.InvalidOperationException);
    }
}