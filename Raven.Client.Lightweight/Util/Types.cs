using System;
using Raven35.Json.Linq;

namespace Raven35.Client.Util
{
    public static class Types
    {
        public static bool IsEntityType(this Type type)
        {
            return type != typeof (object) && type != typeof (RavenJObject);
        }
    }
}
