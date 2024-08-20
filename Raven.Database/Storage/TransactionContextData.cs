using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven35.Database.Storage
{
    public class TransactionContextData
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NumberOfActionsAfterCommit { get; set; }
        public bool IsAlreadyInContext { get; set; }
        public List<string> DocumentIdsToTouch { get; set; }
        public List<string> DocumentTombstonesToTouch { get; set; }
    }
}
