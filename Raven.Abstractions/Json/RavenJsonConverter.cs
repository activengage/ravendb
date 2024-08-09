using System;
using System.Linq;
using Raven35.Imports.Newtonsoft.Json;

namespace Raven35.Abstractions.Json
{
    public abstract class RavenJsonConverter : JsonConverter
    {
        protected object DeferReadToNextConverter(JsonReader reader, Type objectType, JsonSerializer serializer, object existingValue)
        {
            var anotherConverter = serializer.Converters
                .Skip(serializer.Converters.IndexOf(this) + 1)
                .FirstOrDefault(x => x.CanConvert(objectType));
            if (anotherConverter != null)
                return anotherConverter.ReadJson(reader, objectType, existingValue, serializer);
            return reader.Value;
        }
    }
}
