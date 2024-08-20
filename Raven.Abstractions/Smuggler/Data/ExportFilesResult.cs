using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven35.Abstractions.Smuggler.Data
{
    public class ExportFilesResult : LastFilesEtagsInfo
    {
        public string FilePath { get; set; }
    }
}
