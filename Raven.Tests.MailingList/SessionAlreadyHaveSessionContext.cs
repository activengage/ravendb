// -----------------------------------------------------------------------
//  <copyright file="SessionAlreadyHaveSessionContext.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Database.Impl.DTC;
using Raven35.Storage.Esent;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class SessionAlreadyHaveSessionContext : RavenTest
    {
        [Fact]
        public void Repro()
        {
            using (var store = NewDocumentStore(requestedStorage: "esent"))
            {
                var inFlightTransactionalState = store.SystemDatabase.InFlightTransactionalState as EsentInFlightTransactionalState;
                if (inFlightTransactionalState == null)
                    return;
                var transactionalStorage = (TransactionalStorage)store.SystemDatabase.TransactionalStorage;
                using (var context = inFlightTransactionalState.CreateEsentTransactionContext())
                {
                    using (transactionalStorage.SetTransactionContext(context))
                    {
                        using (context.EnterSessionContext())
                        {

                            transactionalStorage.Batch(accessor =>
                            {
                                transactionalStorage.Batch(x =>
                                {

                                });
                            });

                            transactionalStorage.Batch(accessor =>
                            {

                            });
                        }
                    }
                }
            }
        }
    }
}
