using System;
using System.Linq;
using Raven35.Abstractions.Extensions;
using Raven35.Client.Document;
using Raven35.Client.Indexes;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList.spokeypokey
{
    public class Spokey5 : RavenTest
    {
        private class Reference
        {
            public string InternalId { get; set; }
            public string Name { get; set; }
        }

        private class TaxonomyCode : Reference
        {
            public string Code { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime EffectiveThrough { get; set; }
        }

        private class Provider1
        {
            public string InternalId { get; set; }
            public string Name { get; set; }
            public Reference TaxonomyCodeRef { get; set; }
        }

        private class ProviderAndTaxonomyCodeIndex1 :
            AbstractMultiMapIndexCreationTask<ProviderAndTaxonomyCodeIndex1.ProviderTestDto>
        {
            public class ProviderTestDto
            {
                public string InternalId { get; set; }
                public string Name { get; set; }
                public DateTime TaxonomyCode_EffectiveFrom { get; set; }
                public DateTime TaxonomyCode_EffectiveThrough { get; set; }
                public string TaxonomyCode_InternalId { get; set; }
            }

            public ProviderAndTaxonomyCodeIndex1()
            {
                AddMap<Provider1>(
                    providers => from provider in providers
                                 select new
                                 {
                                     InternalId = provider.InternalId,
                                     provider.Name,
                                     TaxonomyCode_EffectiveFrom = DateTime.MinValue,
                                     TaxonomyCode_EffectiveThrough = DateTime.MinValue,
                                     TaxonomyCode_InternalId = provider.TaxonomyCodeRef.InternalId,
                                 }
                    );

                AddMap<TaxonomyCode>(
                    codes => from code in codes
                             select new
                             {
                                 InternalId = (string)null,
                                 Name = (string)null,
                                 TaxonomyCode_EffectiveFrom = code.EffectiveFrom,
                                 TaxonomyCode_EffectiveThrough = code.EffectiveThrough,
                                 TaxonomyCode_InternalId = code.InternalId,
                             }
                    );

                Reduce = results => from r in results
                                    group r by r.TaxonomyCode_InternalId
                                        into g
                                        select new
                                        {
                                            InternalId = g.Select(x => x.InternalId).FirstOrDefault(x => x != null),
                                            Name = g.Select(x => x.Name).FirstOrDefault(x => x != null),
                                            TaxonomyCode_EffectiveFrom = g.Select(x => x.TaxonomyCode_EffectiveFrom).FirstOrDefault(x => x > DateTime.MinValue),
                                            TaxonomyCode_EffectiveThrough = g.Select(x => x.TaxonomyCode_EffectiveThrough).FirstOrDefault(x => x > DateTime.MinValue),
                                            TaxonomyCode_InternalId = g.Key,
                                        };
            }
        }


        [Fact]
        public void CanReferenceChildDocumentsInIndex()
        {
            using(GetNewServer())
            using (var store = new DocumentStore { Url = "http://localhost:8079" }.Initialize())
            {
                store.Conventions.FindIdentityProperty = (x => x.Name == "InternalId");
                new ProviderAndTaxonomyCodeIndex1().Execute(store);

                var taxonomyCode1 = new TaxonomyCode
                {
                    EffectiveFrom = new DateTime(2011, 1, 1),
                    EffectiveThrough = new DateTime(2011, 2, 1),
                    InternalId = "taxonomycodetests/1",
                    Name = "ANESTHESIOLOGY",
                    Code = "207L00000X",
                };
                var taxonomyCode2 = new TaxonomyCode
                {
                    EffectiveFrom = new DateTime(2011, 2, 1),
                    EffectiveThrough = new DateTime(2011, 3, 1),
                    InternalId = "taxonomycodetests/2",
                    Name = "FAMILY PRACTICE",
                    Code = "207Q00000X",
                };
                var provider1 = new Provider1
                {
                    Name = "Joe Schmoe",
                    TaxonomyCodeRef = new Reference
                    {
                        InternalId = taxonomyCode1.InternalId,
                        Name = taxonomyCode1.Name
                    }
                };

                using (var session = store.OpenSession())
                {
                    session.Store(taxonomyCode1);
                    session.Store(taxonomyCode2);
                    session.Store(provider1);
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var result = session.Query<ProviderAndTaxonomyCodeIndex1.ProviderTestDto, ProviderAndTaxonomyCodeIndex1>()
                        .Customize(x => x.WaitForNonStaleResults())
                        .FirstOrDefault(p => p.Name == provider1.Name);

                    var serverErrors = store.DatabaseCommands.GetStatistics().Errors;
                    try
                    {
                        Assert.Empty(serverErrors);
                    }
                    catch (Exception)
                    {
                        serverErrors.ForEach(Console.WriteLine);
                        throw;
                    }

                    Assert.NotNull(result);
                    Assert.Equal(provider1.Name, result.Name);
                }

                using (var session = store.OpenSession())
                {
                    var result = session.Query<ProviderAndTaxonomyCodeIndex1.ProviderTestDto, ProviderAndTaxonomyCodeIndex1>()
                        .FirstOrDefault(p => p.TaxonomyCode_EffectiveFrom == taxonomyCode1.EffectiveFrom);
                    Assert.NotNull(result);
                    Assert.Equal(taxonomyCode1.EffectiveFrom, result.TaxonomyCode_EffectiveFrom);
                }
            }
        }
    }
}
