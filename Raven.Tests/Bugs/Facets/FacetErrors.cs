using Raven35.Abstractions.Connection;
using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Client.Linq;
using Raven35.Tests.Common.Attributes;
using Raven35.Tests.Common.Dto.Faceted;
using Raven35.Tests.Faceted;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Raven35.Tests.Bugs.Facets
{
    public class FacetErrors : FacetTestBase
    {
        [Fact]
        public void PrestonThinksFacetsShouldNotHideOtherErrors()
        {
            var cameras = GetCameras(30);
            var now = DateTime.Now;

            //putting some in the past and some in the future
            for (int x = 0; x < cameras.Count; x++)
            {
                cameras[x].DateOfListing = now.AddDays(x - 15);
            }


            var dates = new List<DateTime>{
                now.AddDays(-10),
                now.AddDays(-7),
                now.AddDays(0),
                now.AddDays(7)
            };

            using (var store = NewDocumentStore())
            {
                CreateCameraCostIndex(store);

                InsertCameraDataAndWaitForNonStaleResults(store, cameras);

                var facets = new List<Facet>{
                    new Facet
                    {
                        Name = "DateOfListing",
                        Mode = FacetMode.Ranges,
                        Ranges = new List<string>{
                            string.Format("[NULL TO {0:yyyy-MM-ddTHH-mm-ss.fffffff}]", dates[0]),
                            string.Format("[{0:yyyy-MM-ddTHH-mm-ss.fffffff} TO {1:yyyy-MM-ddTHH-mm-ss.fffffff}]", dates[0], dates[1]),
                            string.Format("[{0:yyyy-MM-ddTHH-mm-ss.fffffff} TO {1:yyyy-MM-ddTHH-mm-ss.fffffff}]", dates[1], dates[2]),
                            string.Format("[{0:yyyy-MM-ddTHH-mm-ss.fffffff} TO {1:yyyy-MM-ddTHH-mm-ss.fffffff}]", dates[2], dates[3]),
                            string.Format("[{0:yyyy-MM-ddTHH-mm-ss.fffffff} TO NULL]", dates[3])
                        }
                    }
                };

                var session = store.OpenSession();
                //CameraCostIndex does not include zoom, bad index specified.
                var query = session.Query<Camera, CameraCostIndex>().Where(x => x.Zoom > 3);
                Assert.Throws<System.InvalidOperationException>(() => query.ToList());
                Assert.Throws<ErrorResponseException>(() => query.ToFacets(facets));
            }
        }
    }
}
