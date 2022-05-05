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

        public TestImage VerifyCodeIsAnyOf(params int[] hashCodes)
        {
            var h = this.GetHashCode();

            if (!hashCodes.Contains(h))
            {
                var expected = hashCodes.Length == 1
                    ? hashCodes[0].ToString()
                    : "any of " + string.Join(", ", hashCodes);                

                throw new InvalidOperationException($"Expected {expected} but was: {h}");
            }

            return this;
        }

        #endregion
    }
}
