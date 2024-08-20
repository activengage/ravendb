//-----------------------------------------------------------------------
// <copyright file="MultipleResultsPerDocumentAndPaging.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Raven35.Abstractions.Indexing;
using Raven35.Client;
using Raven35.Client.Document;
using Raven35.Database.Indexing;
using Raven35.Database.Server;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class MultipleResultsPerDocumentAndPaging : RavenTest
    {
        [Fact]
        public void WhenOutputtingMultipleResultsPerDocAndPagingWillGetCorrectSize()
        {
            using (var store = NewDocumentStore())
            {

                store.DatabaseCommands.PutIndex("Movies/ByActor", new IndexDefinition
                {
                    Map = @"
from movie in docs.Movies
from actor in movie.Actors
select new { Actor = actor, Name = movie.Name }",
                    Indexes = { { "Actor", FieldIndexing.Analyzed } },
                    Stores = { { "Name", FieldStorage.Yes } }
                });

                using (var s1 = store.OpenSession())
                {
                    s1.Store(
                        new Movie
                        {
                            Name = "Inception",
                            Actors = new[] { "Leonardo DiCaprio", "Joseph Gordon-Levitt", "Ellen Page", "Tom Hardy", "James Bond", "Shames Bond" }
                        });
                    s1.Store(
                        new Movie
                        {
                            Name = "The Sorcerer's Apprentice",
                            Actors = new[] { "Nicolas Cage", "Jay Baruchel", "Alfred Molina", "Teresa Palmer", "James Bond", "Shames Bond" }
                        });
                    s1.SaveChanges();
                }

                using (var s2 = store.OpenSession())
                {
                    var movies = s2.Advanced.DocumentQuery<Movie>("Movies/ByActor")
                        .WhereEquals("Actor", "Bond")
                        .Take(2)
                        .WaitForNonStaleResults(TimeSpan.FromMinutes(5))
                        .ToList();

                    Assert.Equal(2, movies.Count);
                }
            }
        }


        public class Movie
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public string[] Actors { get; set; }
        }

    }
}
