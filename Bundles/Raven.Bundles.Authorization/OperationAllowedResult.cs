using System.Collections.Generic;

namespace Raven35.Bundles.Authorization
{
    public class OperationAllowedResult
    {
        public bool IsAllowed { get; set; }
        public List<string> Reasons { get; set; }
    }
}
