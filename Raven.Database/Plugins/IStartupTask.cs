//-----------------------------------------------------------------------
// <copyright file="IStartupTask.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.Composition;

namespace Raven35.Database.Plugins
{
    [InheritedExport]
    public interface IStartupTask
    {
        void Execute(DocumentDatabase database);
    }
}
