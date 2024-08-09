//-----------------------------------------------------------------------
// <copyright file="CascadeDeleteTrigger.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition;
using Raven35.Abstractions.Data;
using Raven35.Database.Plugins;
using Raven35.Json.Linq;

namespace Raven35.Bundles.CascadeDelete
{
    [InheritedExport(typeof(AbstractDeleteTrigger))]
    [ExportMetadata("Bundle", "Cascade Delete")]
    [ExportMetadata("IsRavenExternalBundle", true)]
    public class CascadeDeleteTrigger : AbstractDeleteTrigger
    {
        public override void OnDelete(string key, TransactionInformation transactionInformation)
        {
            if (CascadeDeleteContext.IsInCascadeDeleteContext)
                return;

            using (Database.DisableAllTriggersForCurrentThread())
            using (CascadeDeleteContext.Enter())
            {
                RecursiveDelete(key, transactionInformation);
            }
        }

        private void RecursiveDelete(string key, TransactionInformation transactionInformation)
        {
            var document = Database.Documents.Get(key, transactionInformation);
            if (document == null)
                return;

            var documentsToDelete = document.Metadata.Value<RavenJArray>(MetadataKeys.DocumentsToCascadeDelete);
            if (documentsToDelete != null)
            {
                foreach (var documentToDelete in documentsToDelete)
                {
                    var documentId = documentToDelete.Value<string>();
                    if (!CascadeDeleteContext.HasAlreadyDeletedDocument(documentId))
                    {
                        CascadeDeleteContext.AddDeletedDocument(documentId);
                        RecursiveDelete(documentId, transactionInformation);
                        Database.Documents.Delete(documentId, null, transactionInformation);
                    }
                }
            }
            var attachmentsToDelete = document.Metadata.Value<RavenJArray>(MetadataKeys.AttachmentsToCascadeDelete);

            if (attachmentsToDelete != null)
                foreach (var attachmentToDelete in attachmentsToDelete)
                    Database.Attachments.DeleteStatic(attachmentToDelete.Value<string>(), null);
            return;
        }
    }
}
