//-----------------------------------------------------------------------
// <copyright file="SpatialDynamicCompilationExtension.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Lucene.Net.Documents;
using Raven35.Database.Indexing;

namespace Raven35.Database.Plugins.Builtins
{
    public class SpatialDynamicCompilationExtension : AbstractDynamicCompilationExtension
    {
        public override string[] GetNamespacesToImport()
        {
            return new[]
            {
                typeof (SpatialField).Namespace
            };
        }

        public override string[] GetAssembliesToReference()
        {
            return new string[]
            {
                typeof (AbstractField).Assembly.Location
            };
        }
    }
}
