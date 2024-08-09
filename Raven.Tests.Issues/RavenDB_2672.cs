// -----------------------------------------------------------------------
//  <copyright file="RavenDB_2672.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;

using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Client.Linq;
using Raven35.Tests.Common;
using Raven35.Tests.Common.Dto.Faceted;

using Xunit;

namespace Raven35.Tests.Issues
{
    public class RavenDB_2672 : RavenTest
    {
        [Fact]
        public void FacetSearchShouldThrowIfIndexDoesNotExist()
        {
            using (var store = NewDocumentStore())
            {
                using (var session = store.OpenSession())
                {
                    var facets = new List<Facet>
                     {
                        new Facet
                        {
                            Name = "Manufacturer"
                        },
                        new Facet
                        {
                            Name = "Cost_Range",
                            Mode = FacetMode.Ranges,
                            Ranges =
                            {
                                "[NULL TO Dx200.0]",
                                "[Dx200.0 TO Dx400.0]",
                                "[Dx400.0 TO Dx600.0]",
                                "[Dx600.0 TO Dx800.0]",
                                "[Dx800.0 TO NULL]"
                            }
                        },
                        new Facet
                        {
                            Name = "Megapixels_Range",
                            Mode = FacetMode.Ranges,
                            Ranges =
                            {
                                "[NULL TO Dx3.0]",
                                "[Dx3.0 TO Dx7.0]",
                                "[Dx7.0 TO Dx10.0]",
                                "[Dx10.0 TO NULL]"
                            }
                        }
                     };

                    session.Store(new FacetSetup { Id = "facets/CameraFacets", Facets = facets });
                    session.SaveChanges();

                    var e = Assert.Throws<ErrorResponseException>(() => session.Query<Camera>().Where(x => x.Cost >= 100 && x.Cost <= 300).ToFacets("facets/CameraFacets"));
                    Assert.Equal("Index 'dynamic/Cameras' does not exist.", e.Message);

                    e = Assert.Throws<ErrorResponseException>(() => session.Query<Camera>("SomeIndex").Where(x => x.Cost >= 100 && x.Cost <= 300).ToFacets("facets/CameraFacets"));
                    Assert.Equal("Index 'SomeIndex' does not exist.", e.Message);
                }
            }
        }
    }
}
