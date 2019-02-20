using CPR.Core;

namespace CPR.Business.Helpers
{
    public static class ValueExtensions
    {
        public static int? ParseToInt(this string input)
        {
            if (int.TryParse(input, out var result))
                return result;

            return null;
        }

        public static decimal? ParseToDecimal(this string input)
        {
            if (decimal.TryParse(input, out var result))
                return result;

            return null;
        }

        public static string[] GetColumns(this string line)
        {
            return line.Split(GlobalConstants.SEPERATOR);
        }
    }
}