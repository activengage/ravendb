using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven35.Abstractions.Json;
using Raven35.Imports.Newtonsoft.Json.Linq;
using Raven35.Json.Linq;

namespace Raven35.Abstractions.Util
{
    public class IncludesUtil
    {
        private readonly static Regex includePrefixRegex = new Regex(@"(\([^\)]+\))$",
            RegexOptions.Compiled | 
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static IncludePath GetIncludePath(string include)
        {
            var result = new IncludePath { Path = include };
            var match = includePrefixRegex.Match(include);
            if (match.Success && match.Groups.Count >= 2)
            {
                result.Prefix = match.Groups[1].Value;
                result.Path = result.Path.Replace(result.Prefix, "");
                result.Prefix = result.Prefix.Substring(1, result.Prefix.Length - 2);
               
            }
            return result;
        }


        private static void ExecuteInternal(RavenJToken token, string prefix, Func<string, string, bool> loadId)
        {
            if (token == null)
                return; // nothing to do

            switch (token.Type)
            {
                case JTokenType.Array:
                    foreach (var item in (RavenJArray)token)
                    {
                        ExecuteInternal(item, prefix, loadId);
                    }
                    break;
                case JTokenType.String:
                    var value = token.Value<string>();
                    // we need to check on both of them, with id & without id
                    // because people will do products/1 and detaisl/products/1 and want to be able
                    // to include on that
                    loadId(value, prefix);
                    if (prefix != null)
                        loadId(value, null);
                    break;
                case JTokenType.Integer:
                    try
                    {
                        loadId(token.Value<long>().ToString(CultureInfo.InvariantCulture), prefix);
                    }
                    catch (OverflowException)
                    {
                        loadId(token.Value<ulong>().ToString(CultureInfo.InvariantCulture), prefix);
                    }
                    break;
                // here we ignore everything else
                // if it ain't a string or array, it is invalid
                // as an id
            }
        }

        private class IncludePath
        {
            public string Path;
            public string Prefix;
        }


        public static void Include(RavenJObject document, string include, Func<string, bool> loadId)
        {
            if (string.IsNullOrEmpty(include) || document == null)
                return;

            var path = GetIncludePath(include);

            foreach (var token in document.SelectTokenWithRavenSyntaxReturningFlatStructure(path.Path))
            {
                ExecuteInternal(token.Item1, path.Prefix, (value, prefix) =>
                {
                    value = (prefix != null ? prefix + value : value);
                    return loadId(value);
                });
            }
        }
    }
}
