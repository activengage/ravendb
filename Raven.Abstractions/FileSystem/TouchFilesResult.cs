// -----------------------------------------------------------------------
//  <copyright file="TouchFilesResult.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;

namespace Raven35.Abstractions.FileSystem
{
    public class TouchFilesResult
    {
        public long NumberOfProcessedFiles { get; set; }
        public Etag LastProcessedFileEtag { get; set; }
        public long NumberOfFilteredFiles { get; set; }
        public Etag LastEtagAfterTouch { get; set; }
    }
}