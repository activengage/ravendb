using System;

namespace Raven35.Abstractions.Data
{
    public class DatabaseOperationsStatus
    {
        public DateTime? LastBackup { get; set; }

        public DateTime? LastAlertIssued { get; set; }
    }
}
