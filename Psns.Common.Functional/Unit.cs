namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static UnitValue Unit =>
            UnitValue.Default;
    }

    public struct UnitValue
    {
        public static readonly UnitValue Default = new UnitValue();

        public override string ToString() => "()";
    }
}
