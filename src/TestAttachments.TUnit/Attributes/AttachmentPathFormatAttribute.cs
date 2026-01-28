using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TUnit
{
    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Always)]    
    public sealed class AttachmentPathFormatAttribute : PathFormatAttribute
    {
        #region constructors
        
        /// <summary>
        /// Declares a directory formatter to write test attachments.
        /// </summary>
        /// <param name="format">Recommended patterns are: "?" , "*/?" or "*/yourClassName/?" </param>
        public AttachmentPathFormatAttribute(string format) : base("AttachmentPathFormat", format) { }

        #endregion

        #region API
        public static string FindPathFormat(TestContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            return context
                .Metadata
                .TestDetails
                .CustomProperties
                .TryGetValue("AttachmentPathFormat", out var list)
                ? list.FirstOrDefault() // ToDo: we could resolve incrementally
                : null;
        }

        #endregion
    }
}
