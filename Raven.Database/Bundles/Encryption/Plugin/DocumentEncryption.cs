using System.ComponentModel.Composition;
using System.IO;
using Raven35.Bundles.Encryption.Settings;
using Raven35.Database.Bundles.Encryption.Settings;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.Encryption.Plugin
{
    [InheritedExport(typeof(AbstractDocumentCodec))]
    [ExportMetadata("Order", 5000)]
    [ExportMetadata("Bundle", "Encryption")]
    public class DocumentEncryption : AbstractDocumentCodec
    {
        private EncryptionSettings settings;

        public override void Initialize()
        {
            settings = EncryptionSettingsManager.GetEncryptionSettingsForResource(Database);
        }

        public override void SecondStageInit()
        {
            EncryptionSettingsManager.VerifyEncryptionKey(Database, settings);
        }

        public override Stream Encode(string key, RavenJObject data, RavenJObject metadata, Stream dataStream)
        {
            if (EncryptionSettings.DontEncrypt(key))
                return dataStream;

            return settings.Codec.Encode(key, dataStream);
        }

        public override Stream Decode(string key, RavenJObject metadata, Stream dataStream)
        {
            if (EncryptionSettings.DontEncrypt(key))
                return dataStream;

            return settings.Codec.Decode(key, dataStream);
        }

        public void SetSettings(EncryptionSettings newSettings)
        {
            settings = newSettings;
        }
    }
}
