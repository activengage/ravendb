using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven35.Tests.Core.Utils.Transformers
{
    public class UsersTransformer : AbstractTransformerCreationTask<User>
    {
        public class Result
        {
            public string PassedParameter { get; set; }
        }

        public UsersTransformer()
        {
            TransformResults = results => from result in results
                                          let key = ParameterOrDefault("Key", "LastName").Value<string>()
                                          select new { PassedParameter = key };
        }
    }
}
