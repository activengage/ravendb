using System.Collections.Generic;
using Raven35.Abstractions.FileSystem;

namespace Raven35.Database.FileSystem.Synchronization
{
    internal class FileHeaderNameEqualityComparer : IEqualityComparer<FileHeader>
    {
        public static  FileHeaderNameEqualityComparer Instance = new FileHeaderNameEqualityComparer();

        public bool Equals(FileHeader x, FileHeader y)
        {
            return x.FullPath == y.FullPath;
        }

        public int GetHashCode(FileHeader header)
        {
            return (header.FullPath != null ? header.FullPath.GetHashCode() : 0);
        }
    }
}
