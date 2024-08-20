//-----------------------------------------------------------------------
// <copyright file="PatchCommandData.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Json.Linq;

namespace Raven35.Abstractions.Commands
{
    ///<summary>
    /// A single batch operation for a document PATCH
    ///</summary>
    public class PatchCommandData : ICommandData
    {
        /// <summary>
        /// Array of patches that will be applied to the document
        /// </summary>
        public PatchRequest[] Patches { get; set; }

        /// <summary>
        /// Array of patches to apply to a default document if the document is missing
        /// </summary>
        public PatchRequest[] PatchesIfMissing { get; set; }

        /// <summary>
        /// <para>If set to true, _and_ the Etag is specified then the behavior</para>
        /// <para>of the patch in the case of etag mismatch is different. Instead of throwing,</para>
        /// <para>the patch operation wouldn't complete, and the Skipped status would be returned </para>
        /// <para>to the user for this operation</para>
        /// </summary>
        public bool SkipPatchIfEtagMismatch { get; set; }

        /// <summary>
        /// Key of a document to patch.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Returns operation method. In this case PATCH.
        /// </summary>
        public string Method
        {
            get { return "PATCH"; }
        }

        /// <summary>
        /// Current document etag, used for concurrency checks (null to skip check)
        /// </summary>
        public Etag Etag { get; set; }

        /// <summary>
        /// Information used to identify a transaction. Contains transaction Id and timeout.
        /// </summary>
        public TransactionInformation TransactionInformation { get; set; }

        /// <summary>
        /// RavenJObject representing document's metadata.
        /// </summary>
        public RavenJObject Metadata { get; set; }

        /// <summary>
        /// Additional command data. For internal use only.
        /// </summary>
        public RavenJObject AdditionalData { get; set; }

        /// <summary>
        /// Translates this instance to a Json object.
        /// </summary>
        /// <returns>RavenJObject representing the command.</returns>
        public RavenJObject ToJson()
        {
            var ret = new RavenJObject
                    {
                        {"Key", Key},
                        {"Method", Method},
                        {"Patches", new RavenJArray(Patches.Select(x => x.ToJson()))},
                        {"Metadata", Metadata},
                        {"AdditionalData", AdditionalData},
                        {"SkipPatchIfEtagMismatch", SkipPatchIfEtagMismatch}
                    };
            if (Etag != null)
                ret.Add("Etag", Etag.ToString());
            if (PatchesIfMissing != null && PatchesIfMissing.Length > 0)
                ret.Add("PatchesIfMissing", new RavenJArray(PatchesIfMissing.Select(x => x.ToJson())));
            return ret;
        }
    }
}
