using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using TestAttachments;



namespace TUnit
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PathExtensions
    {
        #region API - Resources

        /// <summary>
        /// Gets a <see cref="System.IO.FileInfo"/> for a test file attachment.
        /// </summary>
        /// <param name="context">The current test context</param>
        /// <param name="relativeFilePath"></param>
        /// <returns>A <see cref="System.IO.FileInfo"/> object.</returns>
        public static FINFO GetResourceFileInfo(this TestContext context, params string[] relativeFilePath)
        {
            return context
                .GetResourceDirectoryInfo()
                .ConcatenateToFile(relativeFilePath);
        }

        /// <summary>
        /// Gets a <see cref="System.IO.DirectoryInfo"/> for a test file attachment.
        /// </summary>
        /// <param name="context">The current test context</param>
        /// <param name="relativeFilePath"></param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static DINFO GetResourceDirectoryInfo(this TestContext context, params string[] relativeFilePath)
        {
            return context
                .GetResourceDirectoryInfo()
                .ConcatenateToDirectory(relativeFilePath);
        }

        public static Uri GetResourceDirectoryUri(this TestContext context)
        {
            return context
                .GetResourceDirectoryInfo()
                .ToUri();
        }

        /// <summary>
        /// Gets the resource directory for the current test.
        /// </summary>
        /// <param name="context">The current test context.</param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static DINFO GetResourceDirectoryInfo(this TestContext context)
        {
            return TUnit.ResourcePathFormatAttribute
                .FindPathFormat(context)
                .FormatResourcePath(context)
                .ToDirectoryInfo();
        }

        /// <summary>
        /// Gets the resource directory for the current test.
        /// </summary>
        /// <param name="context">The current type context.</param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static DINFO GetResourceDirectoryInfo(this Type context)
        {
            throw new NotImplementedException();

            /*
            return context
                .FindResourcesPathFormat()
                .FormatResourcePath(context)
                .ToDirectoryInfo();*/
        }

        #endregion

        #region API - Attachments

        /// <summary>
        /// Gets a <see cref="System.IO.FileInfo"/> for a test file attachment.
        /// </summary>
        /// <param name="context">The current test context</param>
        /// <param name="relativeFilePath"></param>
        /// <returns>A <see cref="System.IO.FileInfo"/> object.</returns>
        public static FINFO GetAttachmentFileInfo(this TestContext context, string relativeFilePath)
        {
            return context
                .GetAttachmentDirectoryInfo()
                .ConcatenateToFile(relativeFilePath);
        }

        public static Uri GetAttachmentDirectoryUri(this TestContext context)
        {
            return context
                .GetAttachmentDirectoryInfo()
                .ToUri();
        }

        /// <summary>
        /// Gets the attachments directory for the current test.
        /// </summary>
        /// <param name="context">The current test context.</param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static DINFO GetAttachmentDirectoryInfo(this TestContext context)
        {
            return TUnit.AttachmentPathFormatAttribute
                .FindPathFormat(context)
                .FormatAttachmentPath(context)
                .ToDirectoryInfo();
        }

        #endregion

        
    }
}
