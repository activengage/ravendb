using System;
using Lucene.Net.Search;
using Raven35.Abstractions.Data;

namespace Raven35.Database.Indexing.Sorting.Custom
{
    public class CustomSortField : SortField
    {
        private readonly IndexEntriesToComparablesGenerator generator;
        public CustomSortField(string typeName, IndexQuery indexQuery, bool reverse)
            : base(String.Empty, INT,reverse)
        {
            var clrType = System.Type.GetType(typeName, throwOnError: true);
            generator = (IndexEntriesToComparablesGenerator)Activator.CreateInstance(clrType, new object[] {indexQuery});
        }

        public override FieldComparator GetComparator(int numHits, int sortPos)
        {
            return new CustomSortFieldCompartor(generator, numHits);
        }

        public override FieldComparatorSource ComparatorSource
        {
            get
            {
                return new CustomSortFieldComparatorSource(this);
            }
        }
        private class CustomSortFieldComparatorSource :FieldComparatorSource
        {
            private readonly CustomSortField parent;

            public CustomSortFieldComparatorSource(CustomSortField parent)
            {
                this.parent = parent;
            }

            public override FieldComparator NewComparator(string fieldname, int numHits, int sortPos, bool reversed)
            {
                return new CustomSortFieldCompartor(parent.generator, numHits);
            }
        }
    }
}
