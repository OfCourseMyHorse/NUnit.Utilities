using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NUnit.Framework
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
        public static System.IO.FileInfo GetResourceFileInfo(this TestContext context, params string[] relativeFilePath)
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
        public static System.IO.DirectoryInfo GetResourceDirectoryInfo(this TestContext context, params string[] relativeFilePath)
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
        public static System.IO.DirectoryInfo GetResourceDirectoryInfo(this TestContext context)
        {
            return context
                .FindResourcesPathFormat()
                .FormatResourcePath(context)
                .ToDirectoryInfo();
        }

        /// <summary>
        /// Gets the resource directory for the current test.
        /// </summary>
        /// <param name="context">The current type context.</param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static System.IO.DirectoryInfo GetResourceDirectoryInfo(this Type context)
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
        public static System.IO.FileInfo GetAttachmentFileInfo(this TestContext context, string relativeFilePath)
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
        public static System.IO.DirectoryInfo GetAttachmentDirectoryInfo(this TestContext context)
        {
            return context
                .FindAttachmentPathFormat()
                .FormatAttachmentPath(context)
                .ToDirectoryInfo();
        }

        #endregion

        #region API - Shortcuts

        /// <summary>
        /// Attaches a shortcut link pointing to the current test directory.
        /// </summary>        
        public static void AttachFolderBrowserShortcut(this TestContext context)
        {
            var tdir = context.GetAttachmentDirectoryInfo();

            var t = new AttachmentInfo(context, "📂 Show Directory.url");

            t.WriteObjectEx(f=> ShortcutUtils.CreateLink(f.FullName, tdir.FullName) );
        }

        #endregion
    }
}
