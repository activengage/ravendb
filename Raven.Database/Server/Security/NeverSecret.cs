using System;
using System.Collections.Generic;

namespace Raven35.Database.Server.Security
{
    public class NeverSecret
    {
        public static readonly HashSet<string> Urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // allow to get files that are static and are never secret, for example, the studio, the cross domain
                // policy and the fav icon
                "/",
                "/Raven35.Studio.html",
                "/silverlight/Raven35.Studio.xap",
                "/favicon.ico",
                "/clientaccesspolicy.xml",
                "/build/version",
                "/OAuth/API-Key",
                "/OAuth/Cookie",
            };

        public static bool IsNeverSecretUrl(string requestUrl)
        {
            return Urls.Contains(requestUrl) || IsHtml5StudioUrl(requestUrl);
        }

        private static bool IsHtml5StudioUrl(string requestUrl)
        {
            return requestUrl.StartsWith("/studio/");
        }
    }
}
