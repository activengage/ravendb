// -----------------------------------------------------------------------
//  <copyright file="FileSystemMetrics.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;

namespace Raven35.Abstractions.FileSystem
{
    public class FileSystemMetrics
    {
        public double FilesWritesPerSecond { get; set; }

        public double RequestsPerSecond { get; set; }

        public MeterData Requests { get; set; }

        public HistogramData RequestsDuration { get; set; }
    }
}
