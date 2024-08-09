//-----------------------------------------------------------------------
// <copyright file="JsonToExpando.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using Raven35.Abstractions.Linq;
using Raven35.Database.Linq;
using Raven35.Json.Linq;

namespace Raven35.Database.Json
{
    public static class JsonToExpando
    {
        public static object Convert(RavenJObject obj)
        {
            return new DynamicJsonObject(obj);
        }
    }
}
