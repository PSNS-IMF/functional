namespace Psns.Common.Functional
{
    public static partial class Prelude
    {
        public static Maybe<int> parseInt(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed)
                ? Some(parsed)
                : None;
        }

        public static Maybe<long> parseLong(string value)
        {
            long l;

            return long.TryParse(value, out l)
                ? Some(l)
                : None;
        }

        public static Maybe<double> parseDouble(string value)
        {
            double dbl;

            return double.TryParse(value, out dbl)
                ? Some(dbl)
                : None;
        }
    }
}
