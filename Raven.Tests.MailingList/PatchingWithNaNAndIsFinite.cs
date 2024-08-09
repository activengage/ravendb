// -----------------------------------------------------------------------
//  <copyright file="PatchingWithNaNAndIsFinite.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using Raven35.Abstractions.Data;
using Raven35.Database.Json;
using Raven35.Json.Linq;
using Raven35.Tests.Common;

using Xunit;

namespace Raven35.Tests.MailingList
{
    public class PatchingWithNaNAndIsFinite : NoDisposalNeeded
    {
        [Fact]
        public void ShouldWork()
        {
            var scriptedJsonPatcher = new ScriptedJsonPatcher();
            using (var scope = new DefaultScriptedJsonPatcherOperationScope())
            { 
                var result = scriptedJsonPatcher.Apply(scope, new RavenJObject {{"Val", double.NaN}}, new ScriptedPatchRequest
                {
                    Script = @"this.Finite = isFinite(this.Val);"
                });

            Assert.False(result.Value<bool>("Finite"));
}
        }
    }
}
