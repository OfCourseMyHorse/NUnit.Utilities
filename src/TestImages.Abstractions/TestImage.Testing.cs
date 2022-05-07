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
            return Bgra32.Bitmap.GetStandardDeviation(left.GetBitmapRgba32(), right.GetBitmapRgba32());
        }

        /// <summary>
        /// checks that bitmap hash code is one of the provided hash codes
        /// </summary>
        /// <param name="hashCodes">A collection of valid hash codes.</param>
        /// <returns>true if this hash is included in the collection</returns>
        /// <remarks>
        /// Theoretically a runtime rendered bitmap should only have one hash code,
        /// but there can be subtle rendering differences between machines due to:
        /// Operating Systems, graphics adapters, installed fonts, etc.
        /// So it is safe to check against different codes, based on these variations.
        /// </remarks>
        public bool HashCodeIsAnyOf(params int[] hashCodes)
        {
            return hashCodes.Contains(this.GetHashCode());            
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

        #endregion
    }
}
