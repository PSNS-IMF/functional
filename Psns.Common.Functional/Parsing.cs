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

        public static Maybe<long> ParseLong(string value)
        {
            long l;

            return long.TryParse(value, out l)
                ? Some(l)
                : None;
        }
    }
}
