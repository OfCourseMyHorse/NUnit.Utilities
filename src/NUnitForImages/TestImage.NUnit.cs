using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace NUnitForImages
{
    [RequiresThread(ApartmentState.STA)]
    partial class TestImage
    {
        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private Func<string, string> _LocalTestPathSolver;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private string _LastSavedPath;

        private static string _DefaultTestPathSolver(string fileName)
        {
            var tc = TestContext.CurrentContext;

            var path = System.IO.Path.Combine(tc.WorkDirectory, "AttachedTestImages", tc.Test.ID);

            System.IO.Directory.CreateDirectory(path);

            return System.IO.Path.Combine(path, fileName);
        }

        #endregion

        #region API        

        /// <summary>
        /// Asserts that bitmap hash code is one of the provided hash codes
        /// </summary>
        /// <param name="hashCodes">A collection of valid hash codes.</param>
        /// <returns>Fluent self.</returns>
        /// <remarks>
        /// Theoretically a runtime rendered bitmap should only have one hash code,
        /// but there can be subtle rendering differences between machines due to:
        /// Operating Systems, graphics adapters, installed fonts, etc.
        /// So it is safe to check against different codes, based on these variations.
        /// </remarks>
        public TestImage AssertHashCodeIsAnyOf(params int[] hashCodes)
        {
            CollectionAssert.Contains(hashCodes, this.GetHashCode());
            return this;
        }

        #endregion
    }
}
