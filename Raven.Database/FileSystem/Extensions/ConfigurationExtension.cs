using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Linq;
using Raven35.Database.FileSystem.Storage;
using Raven35.Database.FileSystem.Util;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Abstractions.Extensions;
using Raven35.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Raven35.Database.FileSystem.Extensions
{
    public static class ConfigurationExtension
    {
        public static T GetConfigurationValue<T>(this IStorageActionsAccessor accessor, string key)
        {
            var value = accessor.GetConfig(key);
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
                return value.Value<T>("Value");

            return JsonExtensions.JsonDeserialization<T>(value);
        }

        public static bool TryGetConfigurationValue<T>(this IStorageActionsAccessor accessor, string key, out T result)
        {
            try
            {
                result = GetConfigurationValue<T>(accessor, key);
                return true;
            }
            catch (FileNotFoundException)
            {
                result = default(T);
                return false;
            }
        }

        public static void SetConfigurationValue<T>(this IStorageActionsAccessor accessor, string key, T objectToSave)
        {
            accessor.SetConfig(key, JsonExtensions.ToJObject(objectToSave));
        }
    }
}
