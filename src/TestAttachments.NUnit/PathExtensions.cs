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
        public static System.IO.FileInfo GetResourceFileInfo(this TestContext context, string relativeFilePath)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(relativeFilePath)) throw new ArgumentNullException(nameof(relativeFilePath));

            if (System.IO.Path.IsPathRooted(relativeFilePath)) return new System.IO.FileInfo(relativeFilePath);

            var dir = context.GetResourceDirectoryInfo();
            relativeFilePath = System.IO.Path.Combine(dir.FullName, relativeFilePath);

            return new System.IO.FileInfo(relativeFilePath);
        }

        public static Uri GetResourceDirectoryUri(this TestContext context)
        {
            var dinfo = context.GetResourceDirectoryInfo();
            return new Uri(dinfo.FullName, UriKind.Absolute);
        }

        /// <summary>
        /// Gets the attachments directory for the current test.
        /// </summary>
        /// <param name="context">The current test context.</param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static System.IO.DirectoryInfo GetResourceDirectoryInfo(this TestContext context)
        {
            var format = context.FindResourcesPathFormat();
            var path = format.FormatAttachmentPath(context);

            return new System.IO.DirectoryInfo(path);
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
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(relativeFilePath)) throw new ArgumentNullException(nameof(relativeFilePath));

            if (System.IO.Path.IsPathRooted(relativeFilePath)) return new System.IO.FileInfo(relativeFilePath);

            var dir = context.GetAttachmentDirectoryInfo();            
            relativeFilePath = System.IO.Path.Combine(dir.FullName, relativeFilePath);

            return new System.IO.FileInfo(relativeFilePath);
        }

        public static Uri GetAttachmentDirectoryUri(this TestContext context)
        {
            var dinfo = context.GetAttachmentDirectoryInfo();
            return new Uri(dinfo.FullName, UriKind.Absolute);
        }

        /// <summary>
        /// Gets the attachments directory for the current test.
        /// </summary>
        /// <param name="context">The current test context.</param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static System.IO.DirectoryInfo GetAttachmentDirectoryInfo(this TestContext context)
        {
            var format = context.FindAttachmentPathFormat();
            var path = format.FormatAttachmentPath(context);

            return new System.IO.DirectoryInfo(path);
        }

        #endregion

        #region API - Shortcuts

        /// <summary>
        /// Attaches a shortcut link pointing to the current test directory.
        /// </summary>        
        public static void AttachFolderBrowserShortcut(this TestContext context)
        {
            var tdir = context.GetAttachmentDirectoryInfo();

            var t = new AttachmentInfo(context, "📂 Show Directory.lnk");

            t.WriteObjectEx(f=> ShortcutUtils.CreateLink(f.FullName, tdir.FullName) );
        }

        #endregion
    }
}
