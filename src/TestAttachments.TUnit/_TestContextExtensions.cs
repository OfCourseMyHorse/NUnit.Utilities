using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUnit;
using TUnit.Core;

namespace TestAttachments
{
    internal static class _TestContextExtensions
    {
        #region compatibility with NUnit

        /// <summary>
        /// Gets the directory containing the current test assembly.
        /// </summary>
        public static string TestDirectory(this TestContext ctx) => AppContext.BaseDirectory;

        /// <summary>
        /// Gets the directory to be used for outputting files created by this test run.
        /// </summary>
        public static string WorkDirectory(this TestContext ctx) => AppContext.BaseDirectory;

        #endregion
    }
}
