using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Sparrow;
using Voron;

namespace Corax.Queries
{

    unsafe partial struct UnaryMatch<TInner, TValueType>
    {
        public interface IUnaryMatchComparer
        {
            bool Compare(ReadOnlySpan<byte> sx, ReadOnlySpan<byte> sy);
            bool Compare<T>(T sx, T sy) where T : unmanaged;
        }

        [SkipLocalsInit]
        private static int AndWith(ref UnaryMatch<TInner, TValueType> match, Span<long> buffer, int matches)
        {
            var bufferHolder = QueryContext.MatchesRawPool.Rent(sizeof(long) * buffer.Length);
            var innerBuffer = MemoryMarshal.Cast<byte, long>(bufferHolder).Slice(0, buffer.Length);
            Debug.Assert(innerBuffer.Length == buffer.Length);

            var count = match._fillFunc(ref match, innerBuffer);

            var matchesPtr = (long*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(buffer));
            var baseMatchesPtr = (long*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(innerBuffer));
            var result = MergeHelper.And(matchesPtr, buffer.Length, matchesPtr, matches, baseMatchesPtr, count);

            QueryContext.MatchesRawPool.Return(bufferHolder);
            return result;
        }



        [SkipLocalsInit]
        private static int FillFuncSequenceAny<TComparer>(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
            where TComparer : struct, IUnaryMatchComparer
        {
            // If we query unary, we want to find items where at least one element is valid for our conditions, so we look for the first match.
            // For example: Terms [1,2,3] Q: 'where Term < 3'. We check '1 < 3' and thats it.
            // Because the whole process is about comparing values, there is really no reason why we would call this when the condition is a null
            // value, so if we want to compare against a null (which is pretty common) a more optimized version of FillFuncSequenceAny would be needed.

            var searcher = match._searcher;
            var currentType = ((Slice)(object)match._value).AsReadOnlySpan();

            var comparer = default(TComparer);
            var currentMatches = matches;
            int totalResults = 0;
            int storeIdx = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);

                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out var _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    // If we get a null, we just skip it. It will not match.
                    if (type == IndexEntryFieldType.Null)
                    {
                        if (match._operation != UnaryMatchOperation.NotEquals)
                        {
                            // Found a null, item is not elegible.
                            continue;
                        }
                    }                        
                    else if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                    {
                        var iterator = reader.ReadMany(match._fieldId);

                        var isMatch = false;
                        while (iterator.ReadNext())
                        {
                            if (iterator.IsNull)
                            {
                                if (match._operation != UnaryMatchOperation.NotEquals)
                                {
                                    // Found a null, item is not elegible.
                                    continue;
                                }
                            }

                            var analyzedTerm = match._searcher.ApplyAnalyzer(iterator.Sequence, match._fieldId);
                            
                            // If there is any match, then it is a match
                            if (comparer.Compare(currentType, analyzedTerm))
                            {
                                isMatch = true;
                                break;
                            }
                        }
                        if (!isMatch)
                            continue; // No match, item is not elegible.
                    }
                    else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                    {
                        var read = reader.Read(match._fieldId, out var readType, out var resultX);
                        if (read && readType != IndexEntryFieldType.Null)
                        {
                            var analyzedTerm = match._searcher.ApplyAnalyzer(resultX, match._fieldId);
                            if (!read || !comparer.Compare(currentType, analyzedTerm))
                                continue; // Cant read or no match, item is not elegible.
                        }
                        else
                        {
                            if (match._operation != UnaryMatchOperation.NotEquals)
                            {
                                // Found a null, item is not elegible.
                                continue;
                            }
                        }
                    }

                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return storeIdx;
        }

        [SkipLocalsInit]
        private static int FillFuncSequenceAll<TComparer>(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
            where TComparer : struct, IUnaryMatchComparer
        {
            // If we query unary, we want to find items where at all elements has valid conditions, so we look for all the matches.            
            // The typical use of all is on distinct, we want to know if there are any element that matches the negative. 
            // For example: Terms [1,2,3] Q: 'where Term != 3'. We check every item to ensure there is no match with 3 there.
            // Because the whole process is about comparing values, there is really no reason why we would call this when the condition is a null
            // value, so if we want to compare against a null (which is pretty common) a more optimized version of FillFuncSequenceAny would be needed.

            var searcher = match._searcher;
            var currentType = ((Slice)(object)match._value).AsReadOnlySpan();

            var comparer = default(TComparer);
            var currentMatches = matches;
            int totalResults = 0;
            int storeIdx = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);

                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out var _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    // If we get a null, we just skip it. It will not match.

                    if (type == IndexEntryFieldType.Null)
                    {
                        if (match._operation != UnaryMatchOperation.NotEquals)
                        {
                            // Found a null, item is not elegible.
                            continue;
                        }
                    }                        
                    else if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                    {
                        var iterator = reader.ReadMany(match._fieldId);

                        bool isNotMatch = false;
                        while (iterator.ReadNext())
                        {
                            if (iterator.IsNull && match._operation != UnaryMatchOperation.NotEquals)
                            {
                                isNotMatch = true;
                                break;
                            }

                            var analyzedTerm = match._searcher.ApplyAnalyzer(iterator.Sequence, match._fieldId);
                            if (!comparer.Compare(currentType, analyzedTerm))
                            {
                                isNotMatch = true;
                                break;
                            }
                        }
                        if (isNotMatch)
                            continue; // Has nulls or not a match, item is not elegible.
                    }
                    else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                    {
                        var read = reader.Read(match._fieldId, out var readType, out var resultX);
                        if (readType == IndexEntryFieldType.Null)
                        {
                            if (match._operation != UnaryMatchOperation.NotEquals)
                                continue; // item is not elegible.
                        }
                        else
                        {
                            var analyzedTerm = match._searcher.ApplyAnalyzer(resultX, match._fieldId);
                            if (!read || !comparer.Compare(currentType, analyzedTerm))
                                continue; // Cant read or no match, item is not elegible.
                        }
                    }
                    else continue;

                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return storeIdx;
        }


        [SkipLocalsInit]
        private static int FillFuncAllNonNull(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
        {
            // If we query unary, we want to find if all items are null (most likely wont have much positives on lists), but
            // it can certainly be more common in tuples.
            // Since null is an special case, many of the more general comparisons 

            var searcher = match._searcher;
            var currentMatches = matches;
            int totalResults = 0;
            int storeIdx = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);

                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out var _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    if (type == IndexEntryFieldType.Null)
                        continue; // It is null, item is not elegible.

                    if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                    {
                        if (type.HasFlag(IndexEntryFieldType.HasNulls) && !type.HasFlag(IndexEntryFieldType.Empty))
                        {
                            var iterator = reader.ReadMany(match._fieldId);

                            var isNotMatch = false;
                            while (iterator.ReadNext())
                            {
                                // If there is any null, then it is NOT a match
                                if (@iterator.IsNull)
                                {
                                    isNotMatch = true;
                                    break;
                                }
                            }
                            if (isNotMatch)
                                continue;
                        }
                    }
                    else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                    {
                        var readType = reader.GetFieldType(match._fieldId, out var _);
                        if (readType == IndexEntryFieldType.Null)
                            continue;
                    }
                    else
                    {
                        // It is null
                        continue;
                    }

                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return storeIdx;
        }

        [SkipLocalsInit]
        private static int FillFuncAnyNonNull(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
        {
            // If we query unary, we want to find if all items are null (most likely wont have much positives on lists), but
            // it can certainly be more common in tuples.
            // Since null is an special case, many of the more general comparisons 

            var searcher = match._searcher;
            var currentMatches = matches;
            int totalResults = 0;
            int storeIdx = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);

                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out var _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    if (type == IndexEntryFieldType.Null)
                        continue; // It is null, item is not elegible.
                    
                    if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                    {
                        if (type.HasFlag(IndexEntryFieldType.HasNulls) && !type.HasFlag(IndexEntryFieldType.Empty))
                        {
                            var iterator = reader.ReadMany(match._fieldId);

                            bool isMatch = false;
                            while (iterator.ReadNext())
                            {
                                // If there is any non null, then it is a match
                                if (!@iterator.IsNull)
                                {
                                    isMatch = true;
                                    break;
                                }
                            }
                            if (!isMatch)
                                continue; // It is not a match, item is not elegible.
                        }
                        else
                            continue; // It has not nulls, item is not elegible.
                    }
                    else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                    {
                        var readType = reader.GetFieldType(match._fieldId, out var _);
                        if (readType == IndexEntryFieldType.Null)
                            continue; // It is null, item is not elegible.
                    }
                    
                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return storeIdx;
        }



        [SkipLocalsInit]
        private static int FillFuncAllNull(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
        {
            // If we query unary, we want to find if all items are null (most likely wont have much positives on lists), but
            // it can certainly be more common in tuples.
            // Since null is an special case, many of the more general comparisons 

            var searcher = match._searcher;
            var currentMatches = matches;
            int totalResults = 0;
            int storeIdx = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);

                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out var _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    // If it is not a null, then we need to check lots of things.
                    if (type != IndexEntryFieldType.Null)
                    {
                        if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                        {
                            if (type.HasFlag(IndexEntryFieldType.HasNulls) && !type.HasFlag(IndexEntryFieldType.Empty))
                            {
                                var iterator = reader.ReadMany(match._fieldId);
                                var isNotMatch = false;
                                while (iterator.ReadNext())
                                {
                                    if (!@iterator.IsNull)
                                    {
                                        isNotMatch = true;
                                        break;
                                    }
                                }

                                if (isNotMatch)
                                    continue; // It has some non nulls. item is not elegible.
                            }
                            else
                            {
                                // Does not have nulls or it is an empty list... item is not elegible.
                                continue;
                            }
                        }
                        else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                        {
                            var readType = reader.GetFieldType(match._fieldId, out var _);
                            if (readType != IndexEntryFieldType.Null)
                                continue; // item is not elegible.
                        }
                        else
                            continue;
                    }                        

                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return storeIdx;
        }

        [SkipLocalsInit]
        private static int FillFuncAnyNull(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
        {
            // If we query unary, we want to find if all items are null (most likely wont have much positives on lists), but
            // it can certainly be more common in tuples.
            // Since null is an special case, many of the more general comparisons 

            var searcher = match._searcher;
            var currentMatches = matches;
            int totalResults = 0;
            int storeIdx = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);

                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out var _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    // If it is not a null, then we need to check lots of things.
                    if (type != IndexEntryFieldType.Null)
                    {
                        if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                        {
                            if (type.HasFlag(IndexEntryFieldType.HasNulls) && !type.HasFlag(IndexEntryFieldType.Empty))
                            {
                                var iterator = reader.ReadMany(match._fieldId);

                                bool isMatch = false;
                                while (iterator.ReadNext())
                                {
                                    if (iterator.IsNull)
                                    {
                                        isMatch = true;
                                        break;
                                    }
                                }
                                if (!isMatch)
                                    continue; // It is not a match, item is not elegible.
                            }
                            else
                                continue; // It has not nulls, item is not elegible.
                        }
                        else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                        {
                            var readType = reader.GetFieldType(match._fieldId, out var _);
                            if (readType != IndexEntryFieldType.Null)
                                continue; // It has not nulls, item is not elegible.
                        }
                        else
                            continue;
                    }

                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return storeIdx;
        }

        [SkipLocalsInit]
        private static int FillFuncNumericalAny<TComparer>(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
            where TComparer : struct, IUnaryMatchComparer
        {
            var comparer = default(TComparer);
            var searcher = match._searcher;
            var currentMatches = matches;
            int totalResults = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;
            int storeIdx = 0;            

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);
                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    if (TypesHelper.IsInteger<TValueType>())
                    {
                        long currentType = CoherseValueTypeToLong(match._value);
                        if (type == IndexEntryFieldType.Null)
                        {
                            if (match._operation != UnaryMatchOperation.NotEquals)
                            {
                                // Found a null, item is not elegible.
                                continue;
                            }
                        }
                        else if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                        {
                            var iterator = reader.ReadMany(match._fieldId);

                            bool isMatch = false;
                            while (iterator.ReadNext())
                            {
                                if (iterator.IsNull && match._operation != UnaryMatchOperation.NotEquals)
                                    continue; // Item is null, we will try the next.

                                if (comparer.Compare(currentType, iterator.Long))
                                {
                                    isMatch = true;
                                    break;
                                }
                            }
                            if (!isMatch)
                                continue; // Not a match, item is not elegible.
                        }
                        else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                        {
                            var read = reader.Read<long>(match._fieldId, out var readType, out var resultX);
                            if (!read)
                                continue; // Not a match, item is not elegible.

                            if (readType == IndexEntryFieldType.Null && match._operation != UnaryMatchOperation.NotEquals)
                                continue; // Not a match, item is not elegible

                            if (!comparer.Compare(currentType, resultX))
                                continue;
                        }
                    }
                    else if (TypesHelper.IsFloatingPoint<TValueType>())
                    {
                        double currentType = CoherseValueTypeToDouble(match._value);
                        
                        if (type == IndexEntryFieldType.Null)
                        {
                            if (match._operation != UnaryMatchOperation.NotEquals)
                            {
                                // Found a null, item is not elegible.
                                continue;
                            }
                        }
                        else if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                        {
                            var iterator = reader.ReadMany(match._fieldId);

                            bool isMatch = false;
                            while (iterator.ReadNext())
                            {
                                if (iterator.IsNull && match._operation != UnaryMatchOperation.NotEquals)
                                    continue; // Item is null, we will try the next.

                                if (comparer.Compare(currentType, iterator.Double))
                                {
                                    isMatch = true;
                                    break;
                                }
                            }
                            if (!isMatch)
                                continue; // Not a match, item is not elegible.
                        }
                        else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                        {
                            var read = reader.Read<double>(match._fieldId, out var readType, out var resultX);
                            if (!read)
                                continue; // Not a match, item is not elegible.

                            if (readType == IndexEntryFieldType.Null && match._operation != UnaryMatchOperation.NotEquals)
                                continue; // Not a match, item is not elegible.

                            if (!comparer.Compare(currentType, resultX))
                                continue;
                        }
                    }
                    else
                        throw new NotSupportedException($"Type '{typeof(TValueType).Name} is not supported. Only double and float are supported.");

                    // We found a match.
                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return totalResults;
        }

        [SkipLocalsInit]
        private static int FillFuncNumericalAll<TComparer>(ref UnaryMatch<TInner, TValueType> match, Span<long> matches)
            where TComparer : struct, IUnaryMatchComparer
        {
            var comparer = default(TComparer);
            var searcher = match._searcher;
            var currentMatches = matches;
            int totalResults = 0;
            int maxUnusedMatchesSlots = matches.Length >= 64 ? matches.Length / 8 : 1;
            int storeIdx = 0;

            int results;
            do
            {
                var freeMemory = currentMatches.Slice(storeIdx);
                results = match._inner.Fill(freeMemory);
                if (results == 0)
                    return totalResults;

                for (int i = 0; i < results; i++)
                {
                    var reader = searcher.GetReaderFor(freeMemory[i]);
                    var type = reader.GetFieldType(match._fieldId, out _);
                    if (type == IndexEntryFieldType.Invalid)
                        continue;
                    
                    if (type == IndexEntryFieldType.Null)
                    {
                        if (match._operation != UnaryMatchOperation.NotEquals)
                        {
                            // Found a null, item is not elegible.
                            continue;
                        }
                    }
                    else if (TypesHelper.IsInteger<TValueType>())
                    {
                        long currentType = CoherseValueTypeToLong(match._value);
                        if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                        {
                            var iterator = reader.ReadMany(match._fieldId);

                            bool isNotMatch = false;
                            while (iterator.ReadNext())
                            {
                                if (iterator.IsNull)
                                {
                                    isNotMatch = true;
                                    break;
                                }

                                if (!comparer.Compare(currentType, iterator.Long))
                                {
                                    isNotMatch = true;
                                    break;
                                }
                            }
                            if (isNotMatch)
                                continue; // It is not a match, item is not elegible.
                        }
                        else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                        {
                            var read = reader.Read<long>(match._fieldId, out var readType, out var resultX);
                            if (!read || readType == IndexEntryFieldType.Null)
                                continue; // It is null, item is not elegible.
                                          // 
                            if (!comparer.Compare(currentType, resultX))
                                continue; // It is null or not a match, item is not elegible.   
                        }
                        else
                            continue;
                    }
                    else if (TypesHelper.IsFloatingPoint<TValueType>())
                    {
                        double currentType = CoherseValueTypeToDouble(match._value);
                        if (type.HasFlag(IndexEntryFieldType.List) || type.HasFlag(IndexEntryFieldType.TupleList))
                        {
                            var iterator = reader.ReadMany(match._fieldId);

                            bool isNotMatch = false;
                            while (iterator.ReadNext())
                            {
                                if (iterator.IsNull)
                                {
                                    isNotMatch = true;
                                    break;
                                }

                                if (!comparer.Compare(currentType, iterator.Double))
                                {
                                    isNotMatch = true;
                                    break;
                                }
                            }
                            if (isNotMatch)
                                continue; // It is not a match, item is not elegible.
                        }
                        else if (type.HasFlag(IndexEntryFieldType.Tuple) || type.HasFlag(IndexEntryFieldType.Simple))
                        {
                            var read = reader.Read<double>(match._fieldId, out var readType, out var resultX);
                            if (!read || readType == IndexEntryFieldType.Null)
                                continue; // It is null, item is not elegible.
                                          // 
                            if (!comparer.Compare(currentType, resultX))
                                continue; // It is null or not a match, item is not elegible.   
                        }
                        else
                            continue;
                    }
                    else
                        throw new NotSupportedException($"Type '{typeof(TValueType).Name} is not supported. Only double and float are supported.");

                    // We found a match.
                    currentMatches[storeIdx] = freeMemory[i];
                    storeIdx++;
                    totalResults++;
                }
            } while (results >= totalResults + maxUnusedMatchesSlots);

            return totalResults;
        }

        public static UnaryMatch<TInner, TValueType> YieldIsNull(in TInner inner, IndexSearcher searcher, int fieldId, UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any, int take = -1)
        {
            return new UnaryMatch<TInner, TValueType>(
                in inner, UnaryMatchOperation.Equals,
                searcher, fieldId, default(TValueType),
                mode == UnaryMatchOperationMode.Any ? &FillFuncAnyNull : &FillFuncAllNull, &AndWith,
                inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
        }

        public static UnaryMatch<TInner, TValueType> YieldIsNotNull(in TInner inner, IndexSearcher searcher, int fieldId, UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any, int take = -1)
        {
            return new UnaryMatch<TInner, TValueType>(
                in inner, UnaryMatchOperation.NotEquals,
                searcher, fieldId, default(TValueType),
                mode == UnaryMatchOperationMode.Any ? &FillFuncAnyNonNull : &FillFuncAllNonNull, &AndWith,
                inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
        }

        public static UnaryMatch<TInner, TValueType> YieldGreaterThan(in TInner inner, IndexSearcher searcher, int fieldId, TValueType value, UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any,
            int take = -1)
        {
            if (typeof(TValueType) == typeof(Slice))
            {                
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.GreaterThan,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncSequenceAny<GreaterThanMatchComparer> : &FillFuncSequenceAll<GreaterThanMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
            else
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.GreaterThan,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncNumericalAny<GreaterThanMatchComparer> : &FillFuncNumericalAny<GreaterThanMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
        }

        public static UnaryMatch<TInner, TValueType> YieldGreaterThanOrEqualMatch(in TInner inner, IndexSearcher searcher, int fieldId, TValueType value,
            UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any, int take = -1)
        {
            if (typeof(TValueType) == typeof(Slice))
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.GreaterThanOrEqual,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncSequenceAny<GreaterThanOrEqualMatchComparer> : &FillFuncSequenceAll<GreaterThanOrEqualMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
            else
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.GreaterThanOrEqual,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncNumericalAny<GreaterThanOrEqualMatchComparer> : &FillFuncNumericalAny<GreaterThanOrEqualMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
        }

        public static UnaryMatch<TInner, TValueType> YieldLessThan(in TInner inner, IndexSearcher searcher, int fieldId, TValueType value, UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any,
            int take = -1)
        {
            if (typeof(TValueType) == typeof(Slice))
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.LessThan,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncSequenceAny<LessThanMatchComparer> : &FillFuncSequenceAll<LessThanMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
            else
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.LessThan,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncNumericalAny<LessThanMatchComparer> : &FillFuncNumericalAll<LessThanMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
        }

        public static UnaryMatch<TInner, TValueType> YieldLessThanOrEqualMatch(in TInner inner, IndexSearcher searcher, int fieldId, TValueType value,
            UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any, int take = -1)
        {
            if (typeof(TValueType) == typeof(Slice))
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.LessThanOrEqual,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncSequenceAny<LessThanOrEqualMatchComparer> : &FillFuncSequenceAll<LessThanOrEqualMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
            else
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.LessThanOrEqual,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncNumericalAny<LessThanOrEqualMatchComparer> : &FillFuncNumericalAny<LessThanOrEqualMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
        }

        public static UnaryMatch<TInner, TValueType> YieldNotEqualsMatch(in TInner inner, IndexSearcher searcher, int fieldId, TValueType value,
            UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any, int take = -1)
        {
            if (typeof(TValueType) == typeof(Slice))
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.NotEquals,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncSequenceAny<NotEqualsMatchComparer> : &FillFuncSequenceAll<NotEqualsMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
            else
            {                
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.NotEquals,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncNumericalAny<NotEqualsMatchComparer> : &FillFuncNumericalAny<NotEqualsMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
        }

        public static UnaryMatch<TInner, TValueType> YieldEqualsMatch(in TInner inner, IndexSearcher searcher, int fieldId, TValueType value,
            UnaryMatchOperationMode mode = UnaryMatchOperationMode.Any, int take = -1)
        {
            if (typeof(TValueType) == typeof(Slice))
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.Equals,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncSequenceAny<EqualsMatchComparer> : &FillFuncSequenceAll<EqualsMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
            else
            {
                return new UnaryMatch<TInner, TValueType>(
                    in inner, UnaryMatchOperation.Equals,
                    searcher, fieldId, value,
                    mode == UnaryMatchOperationMode.Any ? &FillFuncNumericalAny<EqualsMatchComparer> : &FillFuncNumericalAny<EqualsMatchComparer>, &AndWith,
                    inner.Count, inner.Confidence.Min(QueryCountConfidence.Normal), mode, take: take);
            }
        }

        internal struct GreaterThanMatchComparer : IUnaryMatchComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare(ReadOnlySpan<byte> sx, ReadOnlySpan<byte> sy)
            {
                return sy.SequenceCompareTo(sx) > 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare<T>(T sx, T sy) where T : unmanaged
            {
                if (typeof(T) == typeof(long))
                    return ((long)(object)sy - (long)(object)sx) > 0;
                if (typeof(T) == typeof(double))
                    return ((double)(object)sy - (double)(object)sx) > 0;

                throw new NotSupportedException($"MatchComparer does not support type {nameof(T)}");
            }
        }

        internal struct GreaterThanOrEqualMatchComparer : IUnaryMatchComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare(ReadOnlySpan<byte> sx, ReadOnlySpan<byte> sy)
            {
                return sy.SequenceCompareTo(sx) >= 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare<T>(T sx, T sy) where T : unmanaged
            {
                if (typeof(T) == typeof(long))
                    return ((long)(object)sy - (long)(object)sx) >= 0;
                if (typeof(T) == typeof(double))
                    return ((double)(object)sy - (double)(object)sx) >= 0;

                throw new NotSupportedException($"MatchComparer does not support type {nameof(T)}");
            }
        }

        internal struct LessThanMatchComparer : IUnaryMatchComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare(ReadOnlySpan<byte> sx, ReadOnlySpan<byte> sy)
            {
                return sy.SequenceCompareTo(sx) < 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare<T>(T sx, T sy) where T : unmanaged
            {
                if (typeof(T) == typeof(long))
                    return ((long)(object)sy - (long)(object)sx) < 0;
                if (typeof(T) == typeof(double))
                    return ((double)(object)sy - (double)(object)sx) < 0;

                throw new NotSupportedException($"MatchComparer does not support type {nameof(T)}");
            }
        }

        internal struct LessThanOrEqualMatchComparer : IUnaryMatchComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare(ReadOnlySpan<byte> sx, ReadOnlySpan<byte> sy)
            {
                return sy.SequenceCompareTo(sx) <= 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare<T>(T sx, T sy) where T : unmanaged
            {
                if (typeof(T) == typeof(long))
                    return ((long)(object)sy - (long)(object)sx) <= 0;
                if (typeof(T) == typeof(double))
                    return ((double)(object)sy - (double)(object)sx) <= 0;

                throw new NotSupportedException($"MatchComparer does not support type {nameof(T)}");
            }
        }

        internal struct NotEqualsMatchComparer : IUnaryMatchComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare(ReadOnlySpan<byte> sx, ReadOnlySpan<byte> sy)
            {
                return sy.SequenceCompareTo(sx) != 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare<T>(T sx, T sy) where T : unmanaged
            {
                if (typeof(T) == typeof(long))
                    return ((long)(object)sy != (long)(object)sx);
                if (typeof(T) == typeof(double))
                    return ((double)(object)sy != (double)(object)sx);

                throw new NotSupportedException($"MatchComparer does not support type {nameof(T)}");
            }
        }

        internal struct EqualsMatchComparer : IUnaryMatchComparer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare(ReadOnlySpan<byte> sx, ReadOnlySpan<byte> sy)
            {
                return sy.SequenceCompareTo(sx) == 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            public bool Compare<T>(T sx, T sy) where T : unmanaged
            {
                if (typeof(T) == typeof(long))
                    return ((long)(object)sy == (long)(object)sx);
                if (typeof(T) == typeof(double))
                    return ((double)(object)sy == (double)(object)sx);

                throw new NotSupportedException($"MatchComparer does not support type {nameof(T)}");
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CoherseValueTypeToLong(TValueType value)
        {
            if (typeof(TValueType) == typeof(long))
                return (long)(object)value;
            if (typeof(TValueType) == typeof(ulong))
                return (long)(ulong)(object)value;
            if (typeof(TValueType) == typeof(int))
                return (long)(int)(object)value;
            if (typeof(TValueType) == typeof(uint))
                return (long)(uint)(object)value;
            if (typeof(TValueType) == typeof(short))
                return (long)(short)(object)value;
            if (typeof(TValueType) == typeof(ushort))
                return (long)(ushort)(object)value;
            if (typeof(TValueType) == typeof(byte))
                return (long)(byte)(object)value;
            if (typeof(TValueType) == typeof(sbyte))
                return (long)(sbyte)(object)value;

            throw new NotSupportedException($"Type '{typeof(TValueType).Name} is not supported. Only long, ulong, int, uint, double and float are supported.");
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CoherseValueTypeToDouble(TValueType value)
        {
            if (typeof(TValueType) == typeof(double))
                return (double)(object)value;
            if (typeof(TValueType) == typeof(float))
                return (double)(float)(object)value;

            throw new NotSupportedException($"Type '{typeof(TValueType).Name} is not supported. Only long, ulong, int, uint, double and float are supported.");
        }

    }
}