// -----------------------------------------------------------------------
//  <copyright file="Class1.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Client.Indexes;

namespace Raven35.Client.Util
{
    public class CSharpClassName
    {
        public static string ConvertToValidClassName(string input)
        {
            var ravenEntityName = input.Replace("-", "_")
                                       .Replace(" ", "_")
                                       .Replace("__", "_");
            if (ExpressionStringBuilder.keywordsInCSharp.Contains(ravenEntityName))
                ravenEntityName += "Item";
            return ravenEntityName;
        }
    }
}
