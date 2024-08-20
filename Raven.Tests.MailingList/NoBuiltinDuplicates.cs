using System;
using System.ComponentModel.Composition.Hosting;
using Raven35.Database;
using Raven35.Database.Config;
using Raven35.Database.Plugins;
using Raven35.Database.Plugins.Builtins;
using Raven35.Database.Plugins.Catalogs;
using Raven35.Tests.Common;

using Xunit;
using System.Linq;

namespace Raven35.Tests.MailingList
{
    public class NoBuiltinDuplicates : NoDisposalNeeded
    {
        [Fact]
        public void ShouldNotHaveDuplicates()
        {
            var compositionContainer = new InMemoryRavenConfiguration
            {
                // can't use that, we have some unloadable assemblies in the base directory, 
                // instead, testing the filtering catalog itself
                Catalog =
                    {
                        Catalogs =
                            {
                                new BuiltinFilteringCatalog(new AssemblyCatalog(typeof(DocumentDatabase).Assembly))
                            }
                    }
            }.Container;

            var enumerable = compositionContainer.GetExportedValues<AbstractReadTrigger>().ToList();
            Assert.Equal(1, enumerable.Count(x => x is FilterRavenInternalDocumentsReadTrigger));
        }
    }
}
