using System;

namespace Raven35.Database.FileSystem.Synchronization
{
    public class DataInfo
    {
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
        public long Length { get; set; }
    }
}
