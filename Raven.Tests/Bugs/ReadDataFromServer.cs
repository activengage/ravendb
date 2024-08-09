//-----------------------------------------------------------------------
// <copyright file="ReadDataFromServer.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using System.Net;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.Bugs
{
    public class ReadDataFromServer : RavenTest
    {
        [Fact]
        public void CanReadDataProperly()
        {
            using(GetNewServer())
            {
                using (var webClient = new WebClient())
                {
                    var downloadData = webClient.DownloadData("http://localhost:8079/" +
                        "indexes?pageSize=128&start=" + "0");
                    var documents = GetString(downloadData);
                    RavenJArray.Parse(documents);
                }
            }
        }

        private static string GetString(byte[] downloadData)
        {
            using (var ms = new MemoryStream(downloadData))
            using (var reader = new StreamReader(ms))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
