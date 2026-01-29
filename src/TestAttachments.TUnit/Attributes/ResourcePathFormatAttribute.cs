using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TUnit
{
    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Always)]    
    public class ResourcePathFormatAttribute : PathFormatAttribute
    {
        private const string _PropertyKey = "ResourcePathFormat";

        #region constructors

        /// <summary>
        /// Declares a directory formatter to read resource files.
        /// </summary>
        /// <param name="format">Recommended patterns is: "*/Resources"</param>
        public ResourcePathFormatAttribute(string format) : base(_PropertyKey, format) { }

        #endregion

        #region API

        public static string FindPathFormat(TestContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return context
                .Metadata
                .TestDetails
                .CustomProperties
                .TryGetValue(_PropertyKey, out var list)
                ? list.FirstOrDefault() // ToDo: we could resolve incrementally
                : null;
        }

        #endregion
    }
}
