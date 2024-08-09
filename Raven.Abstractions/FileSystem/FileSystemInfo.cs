using System;
using System.IO;
using System.Text;
using Raven35.Imports.Newtonsoft.Json;

namespace Raven35.Abstractions.FileSystem
{
    public class FileSystemInfo
    {
        public string Url { get; set; }

        public Guid Id { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Url, Id);
        }

        public string AsJson()
        {
            var sb = new StringBuilder();
            var jw = new JsonTextWriter(new StringWriter(sb));
            new JsonSerializer().Serialize(jw, this);
            return sb.ToString();
        }
    }
}
