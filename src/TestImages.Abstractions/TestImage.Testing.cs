using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestImages
{    
    partial class TestImage
    {
        #region API

        public static double GetStandardDeviation(TestImage left, TestImage right)
        {
            return Bitmaps.Bgra32.Bitmap.GetStandardDeviation(left._GetBitmapRgba32(), right._GetBitmapRgba32());
        }

        /// <summary>
        /// checks that bitmap hash code is one of the provided hash codes
        /// </summary>
        /// <param name="checkSums">A collection of valid hash codes.</param>
        /// <returns>true if this hash is included in the collection</returns>
        /// <remarks>
        /// Theoretically a runtime rendered bitmap should only have one hash code,
        /// but there can be subtle rendering differences between machines due to:
        /// Operating Systems, graphics adapters, installed fonts, etc.
        /// So it is safe to check against different codes, based on these variations.
        /// </remarks>
        [Obsolete("Use CheckSumIsAnyOf")]
        public bool HashCodeIsAnyOf(params int[] checkSums)
        {
            return checkSums.Contains(this.GetHashCode());            
        }

        /// <summary>
        /// checks that bitmap checksum is one of the provided hash codes
        /// </summary>
        /// <param name="checkSums">A collection of valid hash codes.</param>
        /// <returns>true if this hash is included in the collection</returns>
        /// <remarks>
        /// Theoretically a runtime rendered bitmap should only have one hash code,
        /// but there can be subtle rendering differences between machines due to:
        /// Operating Systems, graphics adapters, installed fonts, etc.
        /// So it is safe to check against different codes, based on these variations.
        /// </remarks>
        public bool CheckSumIsAnyOf(params uint[] checkSums)
        {
            return checkSums.Contains(this.CheckSum);
        }

        #if NET6_0_OR_GREATER
        [System.Diagnostics.StackTraceHidden]
        #endif
        public TestImage Assert(Predicate<TestImage> other, string message = null)
        {
            if (!other.Invoke(this)) Exceptions.ReportFail(message);
            return this;
        }

        #if NET6_0_OR_GREATER
        [System.Diagnostics.StackTraceHidden]
        #endif
        [Obsolete("use AssertCheckSumIsAnyOf")]
        public TestImage AssertHashCodeIsAnyOf(params int[] hashCodes)
        {
            var h = this.GetHashCode();

            if (!hashCodes.Contains(h))
            {
                var expected = hashCodes.Length == 1
                    ? hashCodes[0].ToString()
                    : "any of " + string.Join(", ", hashCodes);

                Exceptions.ReportFail($"Expected {expected} but was: {h}");
            }

            return this;
        }

        #if NET6_0_OR_GREATER
        [System.Diagnostics.StackTraceHidden]
        #endif        
        public TestImage AssertCheckSumIsAnyOf(params uint[] checkSums)
        {
            var h = this.CheckSum;

            if (!checkSums.Contains(h))
            {
                var expected = checkSums.Length == 1
                    ? checkSums[0].ToString()
                    : "any of " + string.Join(", ", checkSums);

                Exceptions.ReportFail($"Expected {expected} but was: {h}");
            }

            return this;
        }

        #endregion
    }
}
