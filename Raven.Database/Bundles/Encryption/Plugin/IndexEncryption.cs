using System.ComponentModel.Composition;
using System.IO;
using Raven35.Database.Bundles.Encryption.Settings;
using Raven35.Database.Plugins;
using Raven35.Bundles.Encryption.Streams;
using Raven35.Bundles.Encryption.Settings;
using Raven35.Database;

namespace Raven35.Bundles.Encryption.Plugin
{
    [InheritedExport(typeof(AbstractIndexCodec))]
    [ExportMetadata("Bundle", "Encryption")]
    [ExportMetadata("Order", 10000)]
    public class IndexEncryption : AbstractIndexCodec
    {
        EncryptionSettings settings;

        public override void Initialize(DocumentDatabase database)
        {
            settings = EncryptionSettingsManager.GetEncryptionSettingsForResource(database);
        }

        public override Stream Encode(string key, Stream dataStream)
        {
            // Can't simply use Codec.Encode(key, dataStream) because the resulting stream needs to be seekable

            if (!settings.EncryptIndexes)
                return dataStream;

            return new SeekableCryptoStream(settings, key, dataStream);
        }

        public override Stream Decode(string key, Stream dataStream)
        {
            // Can't simply use Codec.Decode(key, dataStream) because the resulting stream needs to be seekable

            if (!settings.EncryptIndexes)
                return dataStream;

            return new SeekableCryptoStream(settings, key, dataStream);
        }
    }
}
