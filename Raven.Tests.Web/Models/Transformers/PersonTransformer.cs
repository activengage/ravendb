// -----------------------------------------------------------------------
//  <copyright file="PersonTransformer.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
using System.Linq;

using Raven35.Client.Indexes;
using Raven35.Tests.Common.Dto;

namespace Raven35.Tests.Web.Models.Transformers
{
    public class PersonTransformer : AbstractTransformerCreationTask<Person>
    {
        public PersonTransformer()
        {
            TransformResults = results => from result in results select result;
        }
    }
}
