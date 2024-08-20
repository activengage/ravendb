using System.Linq;
using Raven35.Client.Indexes;

namespace Raven35.Tests.Bugs.Indexing
{
    public class Transaction_ByMrn : AbstractIndexCreationTask<Transaction>
    {
        public Transaction_ByMrn()
        {
            Map = transactions => from transaction in transactions
                                  select new
                                  {
                                      MRN =
                                          transaction.MRN
                                  };
        }
    }
}
