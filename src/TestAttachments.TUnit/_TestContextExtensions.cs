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

        #region Resource paths        

        public static DINFO ResolveResourceDirectory(params string[] parts)
        {
            return ResolveResourceDirectory(TestContext.Current, parts);
        }

        public static DINFO ResolveResourceDirectory(this TestContext context, params string[] parts)
        {
            return context.GetResourceDirectoryInfo(parts);
        }

        public static DINFO ResolveResourceDirectory()
        {
            return TestContext.Current.GetResourceDirectoryInfo();
        }

        #endregion


    }
}
