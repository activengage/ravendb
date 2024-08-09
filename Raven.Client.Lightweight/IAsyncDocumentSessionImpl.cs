//-----------------------------------------------------------------------
// <copyright file="IAsyncDocumentSession.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Raven35.Client.Document;
using Raven35.Json.Linq;
using Raven35.Client.Document.Batches;

namespace Raven35.Client
{
    /// <summary>
    /// Interface for document session using async approaches
    /// </summary>
    public interface IAsyncDocumentSessionImpl : IAsyncDocumentSession, IAsyncLazySessionOperations, IAsyncEagerSessionOperations
    {
        DocumentConvention Conventions { get; }

        Task<T[]> LoadAsyncInternal<T>(string[] ids, KeyValuePair<string, Type>[] includes, CancellationToken token = default (CancellationToken));

        Task<T[]> LoadUsingTransformerInternalAsync<T>(string[] ids, KeyValuePair<string, Type>[] includes, string transformer, Dictionary<string, RavenJToken> transformerParameters = null, CancellationToken token = default (CancellationToken));

        Lazy<Task<T[]>> LazyAsyncLoadInternal<T>(string[] ids, KeyValuePair<string, Type>[] includes, Action<T[]> onEval, CancellationToken token = default (CancellationToken));

    }
}
