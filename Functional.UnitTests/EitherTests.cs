using NUnit.Framework;
using Psns.Common.Functional;
using System;
using static NUnit.StaticExpect.Expectations;
using static Psns.Common.Functional.Prelude;

namespace SystemExtensions.UnitTests.Functional
{
    [TestFixture]
    public class EitherTests
    {
        static Func<Either<string, int>>[] nullCases =
        {
            () => Left<string, int>(null)
        };

        [TestCaseSource("nullCases")]
        public void Constructing_WithNullRight_ThrowsArgumentNullException(Func<Either<string, int>> run) =>
            Expect(() => run(), Throws.ArgumentNullException);

        static Either<string, int>[] rightCases =
        {
            (Either<string, int>)1,
            Right<string, int>(1)
        };

        [TestCaseSource("rightCases")]
        public void Constructing_Right_ShouldBeRight(Either<string, int> either) =>
            Expect(Resolve(either), EqualTo(RightVal));

        static Either<string, int>[] leftCases =
        {
            (Either<string, int>)LeftVal,
            Left<string, int>(LeftVal)
        };

        [TestCaseSource("leftCases")]
        public void Constructing_Left_ShouldBeLeft(Either<string, int> either) =>
            Expect(Resolve(either), EqualTo(LeftVal));

        [Test]
        public void Comparing_LeftWithRight_NotEqual() =>
            Expect(Left, NUnit.StaticExpect.Expectations.Not.EqualTo(Right));

        [Test]
        public void Comparing_LeftWithNone_NotEqual() =>
            Expect(Left, NUnit.StaticExpect.Expectations.Not.EqualTo(None));

        [Test]
        public void Comparing_RightWithNone_NotEqual() =>
            Expect(Right, NUnit.StaticExpect.Expectations.Not.EqualTo(None));

        [Test]
        public void Comparing_LeftWithLeft_AreEqual() =>
            Expect(Left, EqualTo(Left));

        [Test]
        public void Comparing_RightWithLeft_AreEqual() =>
            Expect(Right, EqualTo(Right));

        [Test]
        public void Comparing_NoneWithNone_AreEqual() =>
            Expect(None, EqualTo(None));

        [Test]
        public void Binding_LeftWithRight_IsLeft() =>
            Expect(Resolve(Left.Bind<int>(e => Right<string, int>(1))), EqualTo(LeftVal));

        [Test]
        public void Binding_RightWithLeft_IsLeft() =>
            Expect(Resolve(Right.Bind<int>(i => Left<string, int>(LeftVal))), EqualTo(LeftVal));

        [Test]
        public void Binding_LeftWithLeft_IsLeft() =>
            Expect(Resolve(Left.Bind(e => Left<string, int>(e))), EqualTo(LeftVal));

        [Test]
        public void Binding_RightWithRight_IsRight() =>
            Expect(Resolve(Right.Bind(s => Right<string, int>(s))), EqualTo(RightVal));

        [Test]
        public void Binding_RightWithNone_IsNone() =>
            Expect(Right.Bind<int>(_ => new Either<string, int>()), EqualTo(None));

        [Test]
        public void Binding_LeftWithNone_IsNone() =>
            Expect(Left.Bind<string>(_ => new Either<string, int>()), EqualTo(None));

        [Test]
        public void Binding_RightWithNone_ThrowsOnMatch() =>
            Expect(() => Resolve(Right.Bind<int>(s => new Either<string, int>())), Throws.InvalidOperationException);

        [Test]
        public void Binding_LeftWithNone_ThrowsOnMatch() =>
            Expect(() => Resolve(Left.Bind<string>(s => new Either<string, int>())), Throws.InvalidOperationException);

        [Test]
        public void Appending_RightWithLeft_IsLeft() =>
            Expect(Right.Append(Left), EqualTo(Left));

        [Test]
        public void Appending_LeftWithRight_IsLeft() =>
            Expect(Left.Append(Right), EqualTo(Left));

        [Test]
        public void Appending_RightWithRight_IsRight() =>
            Expect(Right.Append(Right), EqualTo(Right));

        [Test]
        public void Appending_NoneWithRight_IsNone() =>
            Expect(None.Append(Right), EqualTo(None));

        [Test]
        public void Appending_RightWithNone_IsNone() =>
            Expect(Right.Append(None), EqualTo(None));

        [Test]
        public void Coalescing_RightWithLeft_IsLeft() =>
            Expect(Right | Left, EqualTo(Right));

        [Test]
        public void Coalescing_LeftWithRight_IsLeft() =>
            Expect(Left | Right, EqualTo(Right));

        [Test]
        public void Coalescing_RightWithRight_IsRight() =>
            Expect(Right | Right, EqualTo(Right));

        const string RightVal = "right";
        const string LeftVal = "left";

        static Either<string, int> Right =>
            Right<string, int>(1);

        static Either<string, int> Left =>
            Left<string, int>(string.Empty);

        static Either<string, int> None =>
            Either<string, int>.None;

        static string Resolve(Either<string, int> either) =>
            either.Match(_ => RightVal, _ => LeftVal);
    }
}