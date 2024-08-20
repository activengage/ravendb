using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven35.Abstractions.FileSystem
{
    public class ItemsPage<T>
    {
        public ItemsPage(IEnumerable<T> items, int total)
        {
            TotalCount = total;
            Items = items.ToList();
        }

        public int TotalCount { get; set; }
        public IList<T> Items { get; set; }
    }
}
