using System.Collections.Generic;
using Raven35.Abstractions.Data;
using Raven35.Json.Linq;

namespace Raven35.Database.Config.Retriever
{
    public class ConfigurationSettings
    {
        public Dictionary<string, ConfigurationSetting> Results { get; set; }
    }

    public class ConfigurationSetting
    {
        public bool LocalExists { get; set; }

        public bool GlobalExists { get; set; }

        public string EffectiveValue { get; set; }

        public string GlobalValue { get; set; }
    }
}
