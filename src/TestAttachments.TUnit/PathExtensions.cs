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
        public static FILEINFO GetResourceFileInfo(this TestContext context, params string[] relativeFilePath)
        {
            return context
                .GetResourceDirectoryInfo()
                .DefineFileInfo(relativeFilePath);
        }

        /// <summary>
        /// Gets a <see cref="DIRINFO"/> for a test file attachment.
        /// </summary>
        /// <param name="context">The current test context</param>
        /// <param name="relativeDirectoryPath"></param>
        /// <returns>A <see cref="DIRINFO"/> object.</returns>
        public static DIRINFO GetResourceDirectoryInfo(this TestContext context, params string[] relativeDirectoryPath)
        {
            return context
                .GetResourceDirectoryInfo()
                .DefineDirectoryInfo(relativeDirectoryPath);
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
        /// <returns>A <see cref="DIRINFO"/> object.</returns>
        public static DIRINFO GetResourceDirectoryInfo(this TestContext context)
        {
            return ResourcePathFormatAttribute
                .FindPathFormat(context)
                .FormatResourcePath(context)
                .ToDirectoryInfo();
        }        

        #endregion

        #region API - Attachments

        /// <summary>
        /// Gets a <see cref="System.IO.FileInfo"/> for a test file attachment.
        /// </summary>
        /// <param name="context">The current test context</param>
        /// <param name="relativeFilePath"></param>
        /// <returns>A <see cref="System.IO.FileInfo"/> object.</returns>
        public static FILEINFO GetAttachmentFileInfo(this TestContext context, string relativeFilePath)
        {
            return context
                .GetAttachmentDirectoryInfo()
                .DefineFileInfo(relativeFilePath);
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
        /// <returns>A <see cref="DIRINFO"/> object.</returns>
        public static DIRINFO GetAttachmentDirectoryInfo(this TestContext context)
        {
            return AttachmentPathFormatAttribute
                .FindPathFormat(context)
                .FormatAttachmentPath(context)
                .ToDirectoryInfo();
        }

        #endregion        
    }
}
