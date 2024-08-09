// -----------------------------------------------------------------------
//  <copyright file="AllExceptionsAreSerializaable.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Client;
using Raven35.Database;
using Xunit;

namespace Raven35.Tests
{
    public class AllExceptionsAreSerializaable  : IDisposable
    {
        [Fact]
        public void AllExceptionsAreGood()
        {
            var asms = new[] {typeof (DocumentDatabase).Assembly, typeof (IDocumentStore).Assembly, typeof (DocumentChangeNotification).Assembly};

            foreach (var assembly in asms)
            {
                var customExceptions = assembly.GetTypes().Where(x=>x.IsSubclassOf(typeof(Exception))).ToArray();

                foreach (var customException in customExceptions)
                {
                    Assert.True(customException.IsSerializable, customException.FullName);
                }
            }
        }
        public void Dispose()
        {
            
        }
    }
}
