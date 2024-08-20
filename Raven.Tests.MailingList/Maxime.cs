using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Embedded;
using Raven35.Client.Indexes;
using Raven35.Client.Linq;
using Raven35.Client.Linq.Indexing;
using Raven35.Database.Indexing;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class Maxime : RavenTest
    {
        [Fact]
        public void WithingRadiusOf_Should_Not_Break_Relevance()
        {
            using (var store = NewDocumentStore())
            using (var session = store.OpenSession())
            {
                new PlacesByTermsAndLocation().Execute(store);

                var place1 = new Place("Universit� du Qu�bec � Montr�al")
                {
                    Id = "places/1",
                    Description = "L'Universit� du Qu�bec � Montr�al (UQAM) est une universit� francophone, publique et urbaine de Montr�al, dans la province du Qu�bec au Canada.",
                    Latitude = 45.50955,
                    Longitude = -73.569131
                };

                var place2 = new Place("UQAM")
                {
                    Id = "places/2",
                    Description = "L'Universit� du Qu�bec � Montr�al (UQAM) est une universit� francophone, publique et urbaine de Montr�al, dans la province du Qu�bec au Canada.",
                    Latitude = 45.50955,
                    Longitude = -73.569131
                };

                session.Store(place1);
                session.Store(place2);

                session.SaveChanges();

                // places/2: perfect match + boost
                var terms = "UQAM";
                RavenQueryStatistics stats;
                var places = session.Advanced.DocumentQuery<Place, PlacesByTermsAndLocation>()
                    .WaitForNonStaleResults()
                    .Statistics(out stats)
                    .WithinRadiusOf(500, 45.54545, -73.63908)
                    .Where("(Name:(" + terms + ") OR Terms:(" + terms + "))")
                    .Take(10)
                    .ToList();

                Assert.Equal("places/2", places[0].Id);
                // places/1: perfect match + boost
                terms = "Universit� Qu�bec Montr�al";
                places = session.Advanced.DocumentQuery<Place, PlacesByTermsAndLocation>()
                    .WaitForNonStaleResults()
                    .Statistics(out stats)
                    .WithinRadiusOf(500, 45.54545, -73.63908)
                    .Where("(Name:(" + terms + ") OR Terms:(" + terms + "))")
                    .Take(10)
                    .ToList();

                Assert.Equal("places/1", places[0].Id);
            }
        }


        [Fact]
        public void Can_just_set_to_sort_by_relevance_without_filtering()
        {
            using (var store = NewDocumentStore())
            using (var session = store.OpenSession())
            {
                new PlacesByTermsAndLocation().Execute(store);

                var place1 = new Place("Universit� du Qu�bec � Montr�al")
                {
                    Id = "places/1",
                    Description = "L'Universit� du Qu�bec � Montr�al (UQAM) est une universit� francophone, publique et urbaine de Montr�al, dans la province du Qu�bec au Canada.",
                    Latitude = 45.50955,
                    Longitude = -73.569131
                };

                var place2 = new Place("UQAM")
                {
                    Id = "places/2",
                    Description = "L'Universit� du Qu�bec � Montr�al (UQAM) est une universit� francophone, publique et urbaine de Montr�al, dans la province du Qu�bec au Canada.",
                    Latitude = 45.50955,
                    Longitude = -73.569131
                };

                session.Store(place1);
                session.Store(place2);

                session.SaveChanges();

                // places/1: perfect match + boost
                const string terms = "Universit� Qu�bec Montr�al";
                RavenQueryStatistics stats;
                var places = session.Advanced.DocumentQuery<Place, PlacesByTermsAndLocation>()
                    .WaitForNonStaleResults()
                    .Statistics(out stats)
                    .RelatesToShape(Constants.DefaultSpatialFieldName, "Point(45.54545 -73.63908)", SpatialRelation.Nearby)
                    .Where("(Name:(" + terms + ") OR Terms:(" + terms + "))")
                    .Take(10)
                    .ToList();

                Assert.Equal("places/1", places[0].Id);
                Assert.Equal("places/2", places[1].Id);
            }
        }

        public class Place
        {
            public Place(string name)
            {
                Name = name;
            }

            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Address { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public class PlacesByTermsAndLocation : AbstractIndexCreationTask<Place, PlacesByTermsAndLocation.PlaceQuery>
        {
            public class PlaceQuery
            {
                public string Name { get; set; }
                public string Terms { get; set; }
            }

            public PlacesByTermsAndLocation()
            {
                Map = boards =>
                      from b in boards
                      select new
                      {
                          Name = b.Name.Boost(3),
                          Terms = new
                          {
                              b.Description,
                              b.Address
                          },
                          _ = SpatialGenerate(b.Latitude, b.Longitude)
                      };

                Index(p => p.Name, FieldIndexing.Analyzed);
                Index(p => p.Terms, FieldIndexing.Analyzed);

            }
        } 
    }
}
