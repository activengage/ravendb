//-----------------------------------------------------------------------
// <copyright file="TransactionalStorageConfigurator.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Raven35.Database.Config;
using Raven35.Database.Storage.Esent;

namespace Raven35.Storage.Esent
{
    public class TransactionalStorageConfigurator : StorageConfigurator
    {
        private readonly TransactionalStorage transactionalStorage;

        public TransactionalStorageConfigurator(InMemoryRavenConfiguration configuration, TransactionalStorage transactionalStorage)
            : base(configuration)
        {
            this.transactionalStorage = transactionalStorage;
        }

        protected override void ConfigureInstanceInternal(int maxVerPages)
        {
            if (transactionalStorage != null)
            {
                transactionalStorage.MaxVerPagesValueInBytes = maxVerPages * 1024 * 1024;
            }
        }

        protected override string BaseName
        {
            get
            {
                return "RVN";
            }
        }

        protected override string EventSource
        {
            get
            {
                return "Raven";
            }
        }
    }
}
