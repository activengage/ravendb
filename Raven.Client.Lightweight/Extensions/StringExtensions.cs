using System.Globalization;

namespace Raven35.Client.Extensions
{
    internal static class StringExtensions
    {
        public static string ToInvariantString(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString(this decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
