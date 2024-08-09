// -----------------------------------------------------------------------
//  <copyright file="CanParse.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Raven35.Abstractions.Extensions;
using Raven35.Json.Linq;
using Xunit;

namespace Raven35.Tests.Json
{
    public class CanParse : IDisposable
    {
        [Fact]
        public void CanParseJson()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("{'Test': true}"));
                    var ravenJObject = memoryStream.ToJObject();
                    Assert.True(ravenJObject.Value<bool>("Test"));
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        public void Dispose()
        {
            
        }
    }
}
