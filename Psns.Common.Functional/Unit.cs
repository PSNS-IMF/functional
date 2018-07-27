namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static Unit unit =>
            Unit.Default;
    }

    public struct Unit
    {
        public static readonly Unit Default = new Unit();

        public override string ToString() => "()";
    }
}
