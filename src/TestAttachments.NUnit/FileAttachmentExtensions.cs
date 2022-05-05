using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NUnit.Framework
{
    public static class FileAttachmentExtensions
    {
        #region API        

        /// <summary>
        /// Gets a <see cref="System.IO.FileInfo"/> for a test file attachment.
        /// </summary>
        /// <param name="context">The current test context</param>
        /// <param name="relativeFilePath"></param>
        /// <returns>A <see cref="System.IO.FileInfo"/> object.</returns>
        public static System.IO.FileInfo GetAttachmentFileInfo(this TestContext context, string relativeFilePath)
        {
            var dir = context.GetAttachmentDirInfo();            
            relativeFilePath = System.IO.Path.Combine(dir.FullName, relativeFilePath);

            return new System.IO.FileInfo(relativeFilePath);
        }

        /// <summary>
        /// Gets the attachments directory for the current test.
        /// </summary>
        /// <param name="context">The current test context.</param>
        /// <returns>A <see cref="System.IO.DirectoryInfo"/> object.</returns>
        public static System.IO.DirectoryInfo GetAttachmentDirInfo(this TestContext context)
        {
            var format = context.FindAttachmentPathFormat();
            var path = format.FormatPath(context);

            return new System.IO.DirectoryInfo(path);
        }        

        /// <summary>
        /// Attaches a shortcut .url link pointing to the current test directory.
        /// </summary>        
        public static void AttachFolderBrowserShortcut(this TestContext context)
        {
            var tdir = context.GetAttachmentDirInfo();

            var urlContent = ShortcutUtils.CreateLinkContent(tdir.FullName);

            new AttachmentInfo(context, "📂 Show Directory.url").WriteText(urlContent);
        }

        #endregion
    }
}
