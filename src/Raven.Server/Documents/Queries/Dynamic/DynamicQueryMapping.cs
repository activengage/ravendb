﻿using System;
using System.Collections.Generic;
using System.Linq;

using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client.Data;
using Raven.Client.Indexing;
using Raven.Server.Documents.Indexes;
using Raven.Server.Documents.Indexes.Auto;
using Raven.Server.Documents.Indexes.MapReduce;
using Raven.Server.Documents.Queries.Parse;
using Raven.Server.Documents.Queries.Sort;

namespace Raven.Server.Documents.Queries.Dynamic
{
    public class DynamicQueryMapping
    {
        public string ForCollection { get; private set; }

        public DynamicSortInfo[] SortDescriptors { get; private set; } = new DynamicSortInfo[0];

        public DynamicQueryMappingItem[] MapFields { get; private set; } = new DynamicQueryMappingItem[0];

        public string[] GroupByFields { get; private set; } = new string[0];

        public string[] HighlightedFields { get; private set; }

        public bool IsMapReduce { get; private set; }

        public IndexDefinitionBase CreateAutoIndexDefinition()
        {
            if (IsMapReduce == false)
            {
                return new AutoMapIndexDefinition(ForCollection, MapFields.Select(field =>
                    new IndexField
                    {
                        Name = field.Name,
                        Storage = FieldStorage.No,
                        SortOption = SortDescriptors.FirstOrDefault(x => field.Name.Equals(x.Field))?.FieldType,
                        Highlighted = HighlightedFields.Any(x => field.Name.Equals(x))
                    }).ToArray());
            }

            if (MapFields.Length == 0)
                throw new InvalidOperationException("Invalid dynamic map-reduce query mapping. There is no aggregation specified.");

            if (GroupByFields.Length == 0)
                throw new InvalidOperationException("Invalid dynamic map-reduce query mapping. There is no group by field specified.");

            return new AutoMapReduceIndexDefinition(new[] { ForCollection }, MapFields.Select(field =>
                    new IndexField
                    {
                        Name = field.Name,
                        Storage = FieldStorage.Yes,
                        MapReduceOperation = field.MapReduceOperation
                    }).ToArray(),
                    GroupByFields.Select(field =>
                    new IndexField
                    {
                        Name = field,
                        Storage = FieldStorage.Yes,
                    }).ToArray());
        }

        public void ExtendMappingBasedOn(IndexDefinitionBase definitionOfExistingIndex)
        {
            if (IsMapReduce)
                throw new NotSupportedException("TODO arek");

            var extendedMapFields = new List<DynamicQueryMappingItem>(MapFields);

            foreach (var field in definitionOfExistingIndex.MapFields.Values)
            {
                if (extendedMapFields.Any(x => x.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                extendedMapFields.Add(new DynamicQueryMappingItem()
                {
                    Name = field.Name
                });
            }

            MapFields = extendedMapFields.ToArray();

            var extendedSortDescriptors = new List<DynamicSortInfo>(SortDescriptors);

            // TODO iterate once?
            foreach (var field in definitionOfExistingIndex.MapFields.Values)
            {
                if (field.SortOption == null)
                    continue;

                if (extendedSortDescriptors.Any(x => x.Field.Equals(field.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                extendedSortDescriptors.Add(new DynamicSortInfo()
                {
                    Field = field.Name,
                    FieldType = field.SortOption.Value
                });
            }

            //TODO arek - HighlightedFields

            SortDescriptors = extendedSortDescriptors.ToArray();
        }

        public static DynamicQueryMapping Create(string entityName, IndexQuery query)
        {
            var dynamicQueryMapping = new DynamicQueryMapping
            {
                ForCollection = entityName,
                
            };

            IEnumerable<DynamicQueryMappingItem> dynamicMapFields;

            if (query.DynamicMapReduceFields == null)
            {
                // auto map query

                var fields = SimpleQueryParser.GetFieldsForDynamicQuery(query); // TODO arek - not sure if we really need a Tuple<string, string> here

                if (query.SortedFields != null)
                {
                    foreach (var sortedField in query.SortedFields)
                    {
                        var field = sortedField.Field;

                        if (field == Constants.TemporaryScoreValue)
                            continue;

                        if (field.StartsWith(Constants.AlphaNumericFieldName) ||
                            field.StartsWith(Constants.RandomFieldName) ||
                            field.StartsWith(Constants.CustomSortFieldName))
                        {
                            field = SortFieldHelper.CustomField(field).Name;
                        }

                        if (field.EndsWith("_Range"))
                            field = field.Substring(0, field.Length - "_Range".Length);

                        fields.Add(Tuple.Create(SimpleQueryParser.TranslateField(field), field));
                    }
                }

                dynamicMapFields = fields.Select(x => new DynamicQueryMappingItem
                {
                    //From = x.Item1,
                    //To = IndexField.ReplaceInvalidCharactersInFieldName(x.Item2),
                    //QueryFrom = EscapeParentheses(x.Item2),
                    Name = x.Item1.EndsWith("_Range") ? x.Item1.Substring(0, x.Item1.Length - "_Range".Length) : x.Item1
                });

                dynamicQueryMapping.SortDescriptors = GetSortInfo(query.SortedFields,
                    fields.Where(x => x.Item1.EndsWith("_Range")).Select(x => x.Item1).Distinct().ToArray());

                dynamicQueryMapping.HighlightedFields = query.HighlightedFields.EmptyIfNull().Select(x => x.Field).ToArray();
            }
            else
            {
                // dynamic map-reduce query
                // TODO arek: sorted fields

                dynamicQueryMapping.IsMapReduce = true;

                dynamicMapFields = query.DynamicMapReduceFields.Where(x => x.IsGroupBy == false).Select(x => new DynamicQueryMappingItem
                {
                    Name = x.Name,
                    MapReduceOperation = x.OperationType
                });

                dynamicQueryMapping.GroupByFields = query.DynamicMapReduceFields.Where(x => x.IsGroupBy).Select(x => x.Name).ToArray();
            }

            foreach (var dynamicSortInfo in dynamicQueryMapping.SortDescriptors)
            {
                dynamicSortInfo.Field = IndexField.ReplaceInvalidCharactersInFieldName(dynamicSortInfo.Field);
            }

            dynamicQueryMapping.MapFields = dynamicMapFields.OrderByDescending(x => x.Name.Length).ToArray();

            return dynamicQueryMapping;
        }

        public static DynamicSortInfo[] GetSortInfo(SortedField[] sortedFields, string[] numericFields)
        {
            var sortInfo = new List<DynamicSortInfo>();

            foreach (var key in numericFields)
            {
                sortInfo.Add(new DynamicSortInfo
                {
                    Field = key.Substring(0, key.Length - "_Range".Length),
                    FieldType = SortOptions.NumericDefault
                });
            }

            if (sortedFields == null)
                return sortInfo.ToArray();

            foreach (var sortOptions in sortedFields)
            {
                var key = sortOptions.Field;

                if (key.EndsWith("_Range"))
                {
                    sortInfo.Add(new DynamicSortInfo
                    {
                        Field = key.Substring(0, key.Length - "_Range".Length),
                        FieldType = SortOptions.NumericDefault
                    });
                }
                else
                {
                    sortInfo.Add(new DynamicSortInfo
                    {
                        Field = key,
                        FieldType = SortOptions.String
                    });
                }
            }

            return sortInfo.ToArray();
        }
    }
}