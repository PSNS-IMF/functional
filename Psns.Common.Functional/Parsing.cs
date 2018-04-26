namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static Maybe<int> ParseInt(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed)
                ? Some(parsed)
                : None;
        }
    }
}
