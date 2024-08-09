using System;

namespace Raven35.Json.Utilities
{
    internal static class StringUtils
    {
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }
    }
}
