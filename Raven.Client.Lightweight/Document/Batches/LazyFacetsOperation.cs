using System;
using System.Collections.Generic;
using System.Linq;
using Raven35.Abstractions;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Client.Connection;
using Raven35.Client.Connection.Async;
using Raven35.Client.Shard;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Json.Linq;

namespace Raven35.Client.Document.Batches
{
    public class LazyFacetsOperation : ILazyOperation
    {
        private readonly string index;
        private readonly List<Facet> facets;
        private readonly string facetSetupDoc;
        private readonly IndexQuery query;
        private readonly int start;
        private readonly int? pageSize;

        public LazyFacetsOperation( string index, string facetSetupDoc, IndexQuery query, int start = 0, int? pageSize = null ) {
            this.index = index;
            this.facetSetupDoc = facetSetupDoc;
            this.query = query;
            this.start = start;
            this.pageSize = pageSize;
        }

        public LazyFacetsOperation(string index, List<Facet> facets, IndexQuery query, int start = 0, int? pageSize = null)
        {
            this.index = index;
            this.facets = facets;
            this.query = query;
            this.start = start;
            this.pageSize = pageSize;
        }

        public GetRequest CreateRequest()
        {
            string addition;
            if (facetSetupDoc != null)
            {
                addition = "facetDoc=" + facetSetupDoc;
                return new GetRequest
                {
                    Url = "/facets/" + index,
                    Query = string.Format("{0}&facetStart={1}&facetPageSize={2}&{3}",
                                            query.GetQueryString(),
                                            start,
                                            pageSize,
                                            addition)
                };
            }
            var unescapedFacetsJson = AsyncServerClient.SerializeFacetsToFacetsJsonString(facets);
            if (unescapedFacetsJson.Length < (32*1024)-1)
            {
                addition = "facets=" + Uri.EscapeDataString(unescapedFacetsJson);
                return new GetRequest
                {
                    Url = "/facets/" + index,
                    Query = string.Format("{0}&facetStart={1}&facetPageSize={2}&{3}",
                                            query.GetQueryString(),
                                            start,
                                            pageSize,
                                            addition)
                };
            }
            
            return new GetRequest()
            {
                Url = "/facets/" + index,
                Method = "POST",
                Content = unescapedFacetsJson,
                Query = string.Format("{0}&facetStart={1}&facetPageSize={2}",
                                            query.GetQueryString(),
                                            start,
                                            pageSize)
            };

        }

        

        public object Result { get; private set; }
        public QueryResult QueryResult { get; set; }
        public bool RequiresRetry { get; private set; }

        public void HandleResponse(GetResponse response)
        {
            if (response.RequestHasErrors())
            {
                throw new InvalidOperationException("Got an unexpected response code for the request: " + response.Status + "\r\n" + response.Result);
            }

            var result = (RavenJObject)response.Result;
            Result = result.JsonDeserialization<FacetResults>();
        }

        public void HandleResponses(GetResponse[] responses, ShardStrategy shardStrategy)
        {
            var result = new FacetResults();

            foreach (var response in responses.Select(response => (RavenJObject)response.Result))
            {
                var facet = response.JsonDeserialization<FacetResults>();
                foreach (var facetResult in facet.Results)
                {
                    if (!result.Results.ContainsKey(facetResult.Key))
                        result.Results[facetResult.Key] = new FacetResult();

                    var newFacetResult = result.Results[facetResult.Key];
                    foreach (var facetValue in facetResult.Value.Values)
                    {
                        var existingFacetValueRange = newFacetResult.Values.Find((x) => x.Range == facetValue.Range);
                        if (existingFacetValueRange != null)
                            existingFacetValueRange.Hits += facetValue.Hits;
                        else
                            newFacetResult.Values.Add(new FacetValue() { Hits = facetValue.Hits, Range = facetValue.Range });
                    }

                    foreach (var facetTerm in facetResult.Value.RemainingTerms)
                    {
                        if (!newFacetResult.RemainingTerms.Contains(facetTerm))
                            newFacetResult.RemainingTerms.Add(facetTerm);
                    }
                }
            }

            Result = result;
        }

        public IDisposable EnterContext()
        {
            return null;
        }
    }
}
