//-----------------------------------------------------------------------
// <copyright file="HttpExtensions.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Raven35.Imports.Newtonsoft.Json.Linq;
using Raven35.Abstractions.Data;
using Raven35.Json.Linq;

namespace Raven35.Database.Extensions
{
    public static class HttpExtensions
    {
        public static RavenJToken MinimizeToken(RavenJToken obj, int depth = 0)
        {
            switch (obj.Type)
            {
                case JTokenType.Array:
                    var array = new RavenJArray();
                    foreach (var item in ((RavenJArray)obj))
                    {
                        array.Add(MinimizeToken(item, depth + 1));
                    }
                    return array;
                case JTokenType.Object:
                    var ravenJObject = ((RavenJObject)obj);
                    if (ravenJObject.ContainsKey(Constants.Metadata) == false)
                    {
                        // this might be a wrapper object, let check for first level arrays
                        if (depth == 0)
                        {
                            var newRootObj = new RavenJObject();

                            foreach (var prop in ravenJObject)
                            {
                                newRootObj[prop.Key] = prop.Value.Type == JTokenType.Array ?
                                    MinimizeToken(prop.Value, depth + 1) :
                                    prop.Value;
                            }
                            return newRootObj;
                        }
                        return obj;
                    }
                    var newObj = new RavenJObject();
                    newObj[Constants.Metadata] = ravenJObject[Constants.Metadata];
                    return newObj;
                default:
                    return obj;
            }
        }

        public static HttpResponseMessage WithNoCache(this HttpResponseMessage message)
        {
            // Ensure that files are not cached at the browser side.
            // "Cache-Control": "no-cache, no-store, must-revalidate";
            // "Expires": 0;
            message.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MustRevalidate = true,
                NoCache = true,
                NoStore = true,
                MaxAge = new TimeSpan(0, 0, 0, 0)
            };

            return message;
        }
    }
}
