/////////////////////////////////////////////////////////////////////////////
//
// This file was auto-generated by a tool at 2022-12-19 14:55:20
//
// It is recommended you DO NOT directly edit this file but instead edit
// the code-generator that generated this source file instead.
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.Intrinsics.X86.Avx;
using static System.Runtime.Intrinsics.X86.Avx2;
using static System.Runtime.Intrinsics.X86.Sse2;
using static System.Runtime.Intrinsics.X86.Sse41;
using static System.Runtime.Intrinsics.X86.Sse42;

namespace Sparrow.Server.Utils.VxSort
{

    using V = Vector256<float>;
    internal unsafe partial struct Avx2VectorizedSort
    {

        internal struct FloatConfig
        {        
            public const ulong ALIGN = 32;
            public const ulong ALIGN_MASK = ALIGN - 1;
    
            public const long REALIGN_LEFT = 0x666;
            public const long REALIGN_RIGHT = 0x66600000000;
            public const long REALIGN_BOTH = REALIGN_LEFT | REALIGN_RIGHT;        
            
            public const int N = 8;
            
            public const int Unroll = 8;
            public const int SlackPerSideInVectors = Unroll;
            public const int SlackPerSideInElements = SlackPerSideInVectors * N;
            public const int SmallSortThresholdElements = 20 * N;
            
            // The formula for figuring out how much temporary space we need for partitioning:
            // 2 x the number of slack elements on each side for the purpose of partitioning in unrolled manner +
            // 2 x amount of maximal bytes needed for alignment (32)
            // one more vector's worth of elements since we write with N-way stores from both ends of the temporary area
            // and we must make sure we do not accidentally over-write from left -> right or vice-versa right on that edge...
            // In other words, while we allocated this much temp memory, the actual amount of elements inside said memory
            // is smaller by 8 elements + 1 for each alignment (max alignment is actually N-1, I just round up to N...)
            // This long sense just means that we over-allocate N+2 elements...
            public const int PartitionTempSizeInElements = (2 * SlackPerSideInElements + N + 4 * N);   
            public const int PartitionTempSizeInBytes = PartitionTempSizeInElements * sizeof(float);
            public const int ElementAlign = sizeof(float) - 1;    
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float* align_left_scalar_uncommon(float* read_left, float pivot, ref float* tmp_left, ref float* tmp_right)
        {
            /// Called when the left hand side of the entire array does not have enough elements
            /// for us to align the memory with vectorized operations, so we do this uncommon slower alternative.
            /// Generally speaking this is probably called for all the partitioning calls on the left edge of the array
            
            if (((ulong)read_left & FloatConfig.ALIGN_MASK) == 0)
                return read_left;

            var next_align = (float*)(((ulong)read_left + FloatConfig.ALIGN) & ~FloatConfig.ALIGN_MASK);
            while (read_left < next_align)
            {
                var v = *(read_left++);
                if (v <= pivot)
                {
                    *(tmp_left++) = v;
                }
                else
                {
                    *(--tmp_right) = v;
                }
            }

            return read_left;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float* align_right_scalar_uncommon(float* readRight, float pivot, ref float* tmpLeft, ref float* tmpRight)
        {        
            /// Called when the right hand side of the entire array does not have enough elements
            /// for us to align the memory with vectorized operations, so we do this uncommon slower alternative.
            /// Generally speaking this is probably called for all the partitioning calls on the right edge of the array
            
            if (((ulong)readRight & FloatConfig.ALIGN_MASK) == 0)
                return readRight;

            var nextAlign = (float*)((ulong)readRight & ~FloatConfig.ALIGN_MASK);
            while (readRight > nextAlign)
            {
                var v = *(--readRight);
                if (v <= pivot)
                {
                    *(tmpLeft++) = v;
                }
                else
                {
                    *(--tmpRight) = v;
                }
            }

            return readRight;
        }         

 
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void align_vectorized(
            float* left, float* right,
            int leftAlign, int rightAlign,
            in V p,
            byte* pBase,
            ref float* readLeft, ref float* readRight,
            ref float* tmpStartLeft, ref float* tmpLeft,
            ref float* tmpStartRight, ref float* tmpRight) { throw new NotImplementedException(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void partition_block(V dataVec, V p, byte* pBase, ref float* left, ref float* right) { throw new NotImplementedException(); }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void LoadAndPartition1Vectors(float* dataPtr, V P, byte* pBase, ref float* writeLeftPtr, ref float* writeRightPtr)
        {
            // PERF: Unroll and CompressWrite support is been treated as a constant because we make sure the caller
            //       treats that parameter already as a constant @ JIT time causing a cascade.

            var N = V.Count; // Treated as constant @ JIT time

            
            var d1 = LoadAlignedVector256((float*)(dataPtr + N * 0));
            
            partition_block(d1, P, pBase, ref writeLeftPtr, ref writeRightPtr);
        }            


        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void LoadAndPartition4Vectors(float* dataPtr, V P, byte* pBase, ref float* writeLeftPtr, ref float* writeRightPtr)
        {
            // PERF: Unroll and CompressWrite support is been treated as a constant because we make sure the caller
            //       treats that parameter already as a constant @ JIT time causing a cascade.

            var N = V.Count; // Treated as constant @ JIT time

            
            var d4 = LoadAlignedVector256((float*)(dataPtr + N * 0));
            var d3 = LoadAlignedVector256((float*)(dataPtr + N * 1));
            var d2 = LoadAlignedVector256((float*)(dataPtr + N * 2));
            var d1 = LoadAlignedVector256((float*)(dataPtr + N * 3));
            
            partition_block(d4, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d3, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d2, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d1, P, pBase, ref writeLeftPtr, ref writeRightPtr);
        }            


        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void LoadAndPartition8Vectors(float* dataPtr, V P, byte* pBase, ref float* writeLeftPtr, ref float* writeRightPtr)
        {
            // PERF: Unroll and CompressWrite support is been treated as a constant because we make sure the caller
            //       treats that parameter already as a constant @ JIT time causing a cascade.

            var N = V.Count; // Treated as constant @ JIT time

            
            var d8 = LoadAlignedVector256((float*)(dataPtr + N * 0));
            var d7 = LoadAlignedVector256((float*)(dataPtr + N * 1));
            var d6 = LoadAlignedVector256((float*)(dataPtr + N * 2));
            var d5 = LoadAlignedVector256((float*)(dataPtr + N * 3));
            var d4 = LoadAlignedVector256((float*)(dataPtr + N * 4));
            var d3 = LoadAlignedVector256((float*)(dataPtr + N * 5));
            var d2 = LoadAlignedVector256((float*)(dataPtr + N * 6));
            var d1 = LoadAlignedVector256((float*)(dataPtr + N * 7));
            
            partition_block(d8, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d7, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d6, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d5, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d4, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d3, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d2, P, pBase, ref writeLeftPtr, ref writeRightPtr);
            partition_block(d1, P, pBase, ref writeLeftPtr, ref writeRightPtr);
        }            


        
        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private float* vectorized_partition_8(float* left, float* right, long hint)
        {
            Debug.Assert(right - left >= FloatConfig.SmallSortThresholdElements);
            Debug.Assert(((long)left & FloatConfig.ElementAlign) == 0);
            Debug.Assert(((long)right & FloatConfig.ElementAlign) == 0);

            // Vectorized double-pumped (dual-sided) partitioning:
            // We start with picking a pivot using the media-of-3 "method"
            // Once we have sensible pivot stored as the last element of the array
            // We process the array from both ends.
            //
            // To get this rolling, we first read 2 Vector256 elements from the left and
            // another 2 from the right, and store them in some temporary space in order
            // to leave enough "space" inside the vector for storing partitioned values.
            // Why 2 from each side? Because we need n+1 from each side where n is the
            // number of Vector256 elements we process in each iteration... The
            // reasoning behind the +1 is because of the way we decide from *which* side
            // to read, we may end up reading up to one more vector from any given side
            // and writing it in its entirety to the opposite side (this becomes
            // slightly clearer when reading the code below...) Conceptually, the bulk
            // of the processing looks like this after clearing out some initial space
            // as described above:

            // [.............................................................................]
            //  ^wl          ^rl                                               rr^ wr^
            // Where:
            // wl = writeLeft
            // rl = readLeft
            // rr = readRight
            // wr = writeRight

            // In every iteration, we select what side to read from based on how much
            // space is left between head read/write pointer on each side...
            // We read from where there is a smaller gap, e.g. that side
            // that is closer to the unfortunate possibility of its write head
            // overwriting its read head... By reading from THAT side, we're ensuring
            // this does not happen

            // An additional unfortunate complexity we need to deal with is that the
            // right pointer must be decremented by another Vector256<T>.Count elements
            // Since the Load/Store primitives obviously accept start addresses
            var pivot = *right;

            // We do this here just in case we need to pre-align to the right
            // We end up
            *right = float.MaxValue;

            // Broadcast the selected pivot
            var P = Vector256.Create(pivot);

            var readLeft = left;
            var readRight = right;

            

            var tmpStartLeft = (float*)_tempPtr;
            var tmpLeft = tmpStartLeft;
            var tmpStartRight = tmpStartLeft + FloatConfig.PartitionTempSizeInElements;
            var tmpRight = tmpStartRight;

            tmpRight -= FloatConfig.N;

            

            var leftAlign = unchecked((int)(hint & 0xFFFFFFFF));
            var rightAlign = unchecked((int)(hint >> 32));

            var pBase = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(perm_table_32));

            // the read heads always advance by 8 elements, or 32 bytes,
            // We can spend some extra time here to align the pointers
            // so they start at a cache-line boundary
            // Once that happens, we can read with Avx.LoadAlignedVector256
            // And also know for sure that our reads will never cross cache-lines
            // Otherwise, 50% of our AVX2 Loads will need to read from two cache-lines
            align_vectorized(left, right,
                leftAlign, rightAlign, P, pBase,
                ref readLeft, ref readRight,
                ref tmpStartLeft, ref tmpLeft, ref tmpStartRight, ref tmpRight);

            if (leftAlign > 0)
            {
                tmpRight += FloatConfig.N;
                readLeft = align_left_scalar_uncommon(readLeft, pivot, ref tmpLeft, ref tmpRight);
                tmpRight -= FloatConfig.N;
            }

            if (rightAlign < 0)
            {
                tmpRight += FloatConfig.N;
                readRight = align_right_scalar_uncommon(readRight, pivot, ref tmpLeft, ref tmpRight);
                tmpRight -= FloatConfig.N;
            }

            Debug.Assert(((ulong)readLeft & FloatConfig.ALIGN_MASK) == 0);
            Debug.Assert(((ulong)readRight & FloatConfig.ALIGN_MASK) == 0);

            Debug.Assert((((ulong)readRight - (ulong)readLeft) % FloatConfig.ALIGN) == 0);
            Debug.Assert((readRight - readLeft) >= FloatConfig.Unroll * 2);

            // From now on, we are fully aligned
            // and all reading is done in full vector units

            var readLeftV = readLeft;
            var readRightV = readRight;            

            // PERF: This diminished the size of the method and improves the performance. 
            var pointers = stackalloc float*[2];
            pointers[0] = readLeftV;
            pointers[1] = readRightV - FloatConfig.Unroll * FloatConfig.N;
            
            
            
            
            for ( int i = 0; i < 2; i++)
                LoadAndPartition8Vectors(pointers[i], P, pBase, ref tmpLeft, ref tmpRight);

            tmpRight += FloatConfig.N;

            
            

            // Adjust for the reading that was made above
            readLeftV += FloatConfig.N * FloatConfig.Unroll;
            readRightV -= FloatConfig.N * FloatConfig.Unroll * 2;

            float* nextPtr;

            var writeLeft = left;
            var writeRight = right - FloatConfig.N;

            while (readLeftV < readRightV)
            {
                if ((byte*)writeRight - (byte*)readRightV < (2 * (FloatConfig.Unroll * FloatConfig.N) - FloatConfig.N) * sizeof(float))
                {
                    
                    
                    
                    
                    nextPtr = readRightV;
                    readRightV -= FloatConfig.N * FloatConfig.Unroll;
                }
                else
                {
                    
                    
                    
                    
                    // PERF: Ensure that JIT never emits cmov here.
                    nextPtr = readLeftV;
                    readLeftV += FloatConfig.N * FloatConfig.Unroll;
                }

                LoadAndPartition8Vectors(nextPtr, P, pBase, ref writeLeft, ref writeRight);
            }
            
            
            int unrollHalf = FloatConfig.Unroll / 2;
            readRightV += FloatConfig.N * unrollHalf;
            while (readLeftV < readRightV)
            {
                if ((byte*)writeRight - (byte*)readRightV < (2 * (unrollHalf * FloatConfig.N) - FloatConfig.N) * sizeof(float))
                {
                    
                    
                    

                    nextPtr = readRightV;
                    readRightV -= FloatConfig.N * unrollHalf;
                }
                else
                {
                    
                    
                    

                    // PERF: Ensure that JIT never emits cmov here.
                    nextPtr = readLeftV;
                    readLeftV += FloatConfig.N * unrollHalf;
                }

                LoadAndPartition4Vectors(nextPtr, P, pBase, ref writeLeft, ref writeRight);
            }

            readRightV += FloatConfig.N * (unrollHalf - 1);                                                

            
            

            while (readLeftV <= readRightV)
            {
                if ((byte*)writeRight - (byte*)readRightV < FloatConfig.N * sizeof(float))
                {
                    nextPtr = readRightV;
                    readRightV -= FloatConfig.N;                                
                }
                else
                {                   
                    // PERF: Ensure that JIT never emits cmov here.
                    nextPtr = readLeftV;
                    readLeftV += FloatConfig.N;                    
                }

                LoadAndPartition1Vectors(nextPtr, P, pBase, ref writeLeft, ref writeRight);
            }

            
            

            // 3. Copy-back the 4 registers + remainder we partitioned in the beginning
          
            var leftTmpSize = tmpLeft - tmpStartLeft;
            Unsafe.CopyBlockUnaligned(writeLeft, tmpStartLeft, (uint)(leftTmpSize * sizeof(float)));
            writeLeft += leftTmpSize;

            
            var rightTmpSize = tmpStartRight - tmpRight;
            Unsafe.CopyBlockUnaligned(writeLeft, tmpRight, (uint)(rightTmpSize * sizeof(float)));            
           
            
            // Shove to pivot back to the boundary
            *right = *writeLeft;
            *writeLeft++ = pivot;

            

            Debug.Assert(writeLeft > left);
            Debug.Assert(writeLeft <= right + 1);

            return writeLeft;
        }            


        private static void Swap(float* left, float* right)
        {
            var tmp = *left;
            *left = *right;
            *right = tmp;
        }    
        
        private static unsafe void SwapIfGreater(float* leftPtr, float* rightPtr)            
        {
            if (*leftPtr <= *rightPtr) return;
            Swap(leftPtr, rightPtr);
        }
        
        private static unsafe void SwapIfGreater3(in float* leftPtr, in float* middlePtr, in float* rightPtr)
        {
            SwapIfGreater(leftPtr, middlePtr);
            SwapIfGreater(leftPtr, rightPtr);
            SwapIfGreater(middlePtr, rightPtr);
        }
              
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void down_heap(long i, long n, float* lo)
        {
            var d = *(lo + i - 1);
            long child;
            while (i <= n / 2)
            {
                child = 2 * i;
                if (child < n && *(lo + child - 1) < *(lo + child))
                {
                    child++;
                }
                if (!(d < *(lo + child - 1)))
                {
                    break;
                }
                *(lo + i - 1) = *(lo + child - 1);
                i = child;
            }
            *(lo + i - 1) = d;
        }        
        
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        static void heap_sort(float* lo, float* hi)
        {
            long n = hi - lo + 1;
            for (long i = n / 2; i >= 1; i--)
            {
                down_heap(i, n, lo);
            }
            for (long i = n; i > 1; i--)
            {
                Swap(lo, lo + i - 1);
                down_heap(1, i - 1, lo);
            }
        }   


        internal void sort(float* left, float* right, float left_hint, float right_hint, long hint, int depth_limit)
        {
            var length = (int)(right - left + 1);

            float* mid;
            switch (length)
            {
                case 0:
                case 1:
                    return;
                case 2:
                    SwapIfGreater(left, right);
                    return;
                case 3:
                    mid = right - 1;
                    SwapIfGreater(left, mid);
                    SwapIfGreater(left, right);
                    SwapIfGreater(mid, right);
                    return;
            }

            // Go to insertion sort below this threshold
            if (length <= FloatConfig.SmallSortThresholdElements)
            {
                
                
                BitonicSort.Sort(left, length);
                return;
            }

            // Detect a whole bunch of bad cases where partitioning
            // will not do well:
            // 1. Reverse sorted array
            // 2. High degree of repeated values (dutch flag problem, one value)
            if (depth_limit == 0)
            {
                heap_sort(left, right);
                
                return;
            }

            depth_limit--;
            
            
            

            // This is going to be a bit weird:
            // Pre/Post alignment calculations happen here: we prepare hints to the
            // partition function of how much to align and in which direction (pre/post).
            // The motivation to do these calculations here and the actual alignment inside the partitioning code is
            // that here, we can cache those calculations.
            // As we recurse to the left we can reuse the left cached calculation, And when we recurse
            // to the right we reuse the right calculation, so we can avoid re-calculating the same aligned addresses
            // throughout the recursion, at the cost of a minor code complexity
            // Since we branch on the magi values REALIGN_LEFT & REALIGN_RIGHT its safe to assume
            // the we are not torturing the branch predictor.'

            // We use a long as a "struct" to pass on alignment hints to the partitioning
            // By packing 2 32 bit elements into it, as the JIT seem to not do this.
            // In reality  we need more like 2x 4bits for each side, but I don't think
            // there is a real difference'

            var preAlignedLeft = (float*)((ulong)left & ~FloatConfig.ALIGN_MASK);
            var cannotPreAlignLeft = ((long)preAlignedLeft - (long)_startPtr) >> 63;
            var preAlignLeftOffset = (preAlignedLeft - left) + (FloatConfig.N & cannotPreAlignLeft);
            if ((hint & FloatConfig.REALIGN_LEFT) != 0)
            {
                // Alignment flow:
                // * Calculate pre-alignment on the left
                // * See it would cause us an out-of bounds read
                // * Since we'd like to avoid that, we adjust for post-alignment
                // * There are no branches since we do branch->arithmetic
                hint &= unchecked((long)0xFFFFFFFF00000000UL);
                hint |= preAlignLeftOffset;
            }

            var preAlignedRight = (float*)(((ulong)right - 1 & ~FloatConfig.ALIGN_MASK) + FloatConfig.ALIGN);
            var cannotPreAlignRight = ((long)_endPtr - (long)preAlignedRight) >> 63;
            var preAlignRightOffset = (preAlignedRight - right - (FloatConfig.N & cannotPreAlignRight));
            if ((hint & FloatConfig.REALIGN_RIGHT) != 0)
            {
                // right is pointing just PAST the last element we intend to partition (where we also store the pivot)
                // So we calculate alignment based on right - 1, and YES: I am casting to ulong before doing the -1, this
                // is intentional since the whole thing is either aligned to 32 bytes or not, so decrementing the POINTER value
                // by 1 is sufficient for the alignment, an the JIT sucks at this anyway
                hint &= 0xFFFFFFFF;
                hint |= preAlignRightOffset << 32;
            }

            Debug.Assert(((ulong)(left + (hint & 0xFFFFFFFF)) & FloatConfig.ALIGN_MASK) == 0);
            Debug.Assert(((ulong)(right + (hint >> 32)) & FloatConfig.ALIGN_MASK) == 0);

            // Compute median-of-three, of:
            // the first, mid and one before last elements
            mid = left + ((right - left) / 2);
            SwapIfGreater3(left, mid, right - 1);

            // Pivot is mid, place it in the right hand side
            Swap(mid, right);

            
            
            var sep = vectorized_partition_8(left, right, hint);                                

            

            
            
            sort(left, sep - 2, left_hint, *sep, hint | FloatConfig.REALIGN_RIGHT, depth_limit);
            
            sort(sep, right, *(sep - 2), right_hint, hint | FloatConfig.REALIGN_LEFT, depth_limit);
            

            
        }

    }
}
