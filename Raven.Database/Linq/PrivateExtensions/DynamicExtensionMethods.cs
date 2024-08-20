using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

using Raven35.Abstractions.Data;
using Raven35.Abstractions.Linq;

namespace Raven35.Database.Linq.PrivateExtensions
{
    /// <summary>
    /// Extension methods that we are translating on dynamic objects during the 
    /// translation phase of the index compilation
    /// </summary>
    public class DynamicExtensionMethods
    {
        public static dynamic StripHtml(dynamic o)
        {
            if (o == null)
                return new DynamicNullObject();

            var value = o as string;
            if (value == null)
                return new DynamicNullObject();

            if (value == string.Empty) 
                return value;

            var document = new HtmlDocument();
            document.LoadHtml(value);

            return document.DocumentNode.InnerText.Trim();
        }

        public static dynamic ParseInt(dynamic o)
        {
            return ParseInt(o, default(int));
        }

        public static dynamic ParseInt(dynamic o, int defaultValue)
        {
            if (o == null) 
                return defaultValue;

            var value = o as string;
            if (value == null) 
                return defaultValue;

            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        public static dynamic ParseDouble(dynamic o)
        {
            return ParseDouble(o, default(double));
        }

        public static dynamic ParseDouble(dynamic o, double defaultValue)
        {
            if (o == null)
                return defaultValue;

            var value = o as string;
            if (value == null)
                return defaultValue;

            double result;
            return double.TryParse(value, out result) ? result : defaultValue;
        }

        public static dynamic ParseDecimal(dynamic o)
        {
            return ParseDecimal(o, default(decimal));
        }

        public static dynamic ParseDecimal(dynamic o, decimal defaultValue)
        {
            if (o == null)
                return defaultValue;

            var value = o as string;
            if (value == null)
                return defaultValue;

            decimal result;
            return decimal.TryParse(value, out result) ? result : defaultValue;
        }

        public static dynamic ParseShort(dynamic o)
        {
            return ParseShort(o, default(short));
        }

        public static dynamic ParseShort(dynamic o, short defaultValue)
        {
            if (o == null)
                return defaultValue;

            var value = o as string;
            if (value == null)
                return defaultValue;

            short result;
            return short.TryParse(value, out result) ? result : defaultValue;
        }

        public static dynamic ParseLong(dynamic o)
        {
            return ParseLong(o, default(long));
        }

        public static dynamic ParseLong(dynamic o, long defaultValue)
        {
            if (o == null)
                return defaultValue;

            var value = o as string;
            if (value == null)
                return defaultValue;

            long result;
            return long.TryParse(value, out result) ? result : defaultValue;
        }

        public static BoostedValue Boost(dynamic o, object value)
        {
            return new BoostedValue
            {
                Value = o,
                Boost = Convert.ToSingle(value)
            };
        }

        public static object IfEntityIs(dynamic o, string entityName)
        {
            if (string.Equals(o[Constants.Metadata][Constants.RavenEntityName], entityName, StringComparison.OrdinalIgnoreCase))
                return o;
            return new DynamicNullObject();
        }

        public static object Reverse(object o)
        {
            if (o == null)
                return new DynamicNullObject();

            var s = o as string;
            if (s != null)
                return Reverse(s);

            return Reverse((IEnumerable<object>) o);
        }

         private static string Reverse(string str)
         {
            var stringBuilder = new StringBuilder(str.Length);
            for (int i = str.Length-1; i >= 0; i--)
            {
                stringBuilder.Append(str[i]);
            }
            return stringBuilder.ToString();
         }

         private static IEnumerable<dynamic> Reverse(IEnumerable<object> self)
         {
// ReSharper disable InvokeAsExtensionMethod
            return Enumerable.Reverse(self);
// ReSharper restore InvokeAsExtensionMethod
         }
    }
}
