using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Represents an attachement file object.
    /// </summary>
    /// <remarks>
    /// <para>File is only attached to the test after it's been written.</para>
    /// <para>Use <see cref="AttachmentPathFormatAttribute"/> at the top of the test class to define the attachment directory path</para>    
    /// </remarks>
    [System.Diagnostics.DebuggerDisplay("{File.FullName}")]
    public class AttachmentInfo
    {
        #region lifecycle

        public static IEnumerable<System.IO.FileInfo> AttachDirectoryFiles(string directory, string mask="*", SearchOption soption = SearchOption.AllDirectories)
        {
            var dinfo = TestContext.CurrentContext.GetAttachmentDirectoryInfo();

            if (!string.IsNullOrWhiteSpace(directory))
            {
                directory = System.IO.Path.IsPathRooted(directory)
                    ? directory
                    : System.IO.Path.Combine(dinfo.FullName, directory);

                dinfo = new System.IO.DirectoryInfo(directory);
            }

            foreach (var f in dinfo.EnumerateFiles(mask, soption))
            {                
                yield return f;
                TestContext.AddTestAttachment(f.FullName);
            }
        }

        public static AttachmentInfo From(string fileName, string description = null) { return new AttachmentInfo(fileName, description); }

        public AttachmentInfo(string fileName, string description = null)
            : this(TestContext.CurrentContext, fileName, description) { }

        public AttachmentInfo(TestContext context, string fileName, string description = null)
        {
            _WriteShowDirectoryLink = context.FindAttachmentShowDirectoryLinkEnabled();

            File = context.GetAttachmentFileInfo(fileName);
            Description = description;
        }

        #endregion

        #region data

        private bool _WriteShowDirectoryLink;

        public System.IO.FileInfo File { get; }
        public string Description { get; set; }

        #endregion

        #region core

        private void _BeginWriteAttachment()
        {
            if (_WriteShowDirectoryLink)
            {
                var ainfo = From("📂 Show Directory.url");
                ainfo._WriteShowDirectoryLink = false; // prevent reentrancy
                ainfo.WriteLink(File.Directory.FullName);
            }

            File.Directory.Create();
        }

        private System.IO.FileInfo _EndWriteAttachment()
        {
            if (File.Exists) TestContext.AddTestAttachment(File.FullName, Description);

            return File;
        }

        #endregion

        #region API

        /// <summary>
        /// This is a special callback used to consume <see cref="AttachmentInfo"/>
        /// without importing this library dependency.
        /// </summary>
        /// <param name="ainfo">A <see cref="AttachmentInfo"/> instance.</param>
        public static implicit operator Action<Action<System.IO.FileInfo>>(AttachmentInfo ainfo)
        {
            return action => ainfo.WriteObjectEx(action);
        }

        [Obsolete("Use WriteObject or WriteObjectEx", true)]
        public System.IO.FileInfo WriteFile(Action<System.IO.FileInfo> writeAction)
        {
            return WriteObjectEx(writeAction);
        }

        public System.IO.FileInfo WriteObject(Action<string> writeAction)
        {
            return WriteObjectEx(f => writeAction(f.FullName));
        }
        
        public System.IO.FileInfo WriteObjectEx(Action<System.IO.FileInfo> writeAction)
        {
            if (writeAction == null) throw new ArgumentNullException(nameof(writeAction));

            _BeginWriteAttachment();
            writeAction(File);
            return _EndWriteAttachment();
        }

        public System.IO.FileInfo WriteToStream(Action<System.IO.Stream> writeAction)
        {
            _BeginWriteAttachment();

            using (var stream = File.Create())
            {
                writeAction(stream);
            }

            return _EndWriteAttachment();
        }

        public System.IO.Stream CreateStream()
        {
            // we need to create a file so we can attach it.
            // another strategy would be to create a stream derived class that would do the attachment on stream close.            

            return WriteAllBytes(Array.Empty<Byte>()).Create();
        }

        public System.IO.FileInfo WriteAllBytes(ReadOnlySpan<Byte> byteContent)
        {
            _BeginWriteAttachment();

            using (var stream = File.Create())
            {
                #if NETSTANDARD2_0
                foreach (var b in byteContent) stream.WriteByte(b);
                #else
                stream.Write(byteContent);
                #endif
            }

            return _EndWriteAttachment();
        }        

        public System.IO.FileInfo WriteTextLines(params String[] textLines)
        {
            var text = string.Join("\r\n", textLines.Select(line => line == null ? string.Empty : line) );
            return WriteAllText(text);
        }

        public System.IO.FileInfo WriteAllText(String textContent)
        {
            if (textContent == null) textContent = string.Empty;

            void writeText(System.IO.FileInfo finfo)
            {
                System.IO.File.WriteAllText(finfo.FullName, textContent);
            }

            return WriteObjectEx(writeText);
        }        

        public System.IO.FileInfo WriteLink(string fileOrDirectoryPath)
        {
            return this.WriteObjectEx(f => ShortcutUtils.CreateLink(f.FullName, fileOrDirectoryPath));
        }

        #endregion

        #region API

        public static void AttachTOC()
        {
            _AttachTocHtml();
        }

        private static void _AttachTocMarkdown()
        {
            var toc = From("📂 Table of Contents.md");
            var buri = new Uri(toc.File.Directory.FullName + "\\", UriKind.Absolute);

            var attachments = TestExecutionContext.CurrentContext.CurrentResult.TestAttachments;

            var md = new StringBuilder();
            foreach (var attachment in attachments)
            {
                var key = System.IO.Path.GetFileName(attachment.FilePath);
                var val = new Uri(attachment.FilePath, UriKind.Absolute);

                val = buri.MakeRelativeUri(val);

                md.AppendLine($"- [{key}]({val})");
            }

            toc.WriteAllText(md.ToString());
        }

        private static void _AttachTocHtml()
        {
            var toc = From("📂 Table of Contents.html");
            var buri = new Uri(toc.File.Directory.FullName + "\\", UriKind.Absolute);

            var attachments = TestExecutionContext.CurrentContext.CurrentResult.TestAttachments;

            var html = new StringBuilder();

            html.AppendLine("<html>");
            html.AppendLine("<body>");
            html.AppendLine("<ul>");

            foreach (var attachment in attachments)
            {
                var key = System.IO.Path.GetFileName(attachment.FilePath);
                var val = new Uri(attachment.FilePath, UriKind.Absolute);

                val = buri.MakeRelativeUri(val);

                html.AppendLine($"<li><a href=\"{val}\">{key}</a></li>");
            }

            html.AppendLine("</ul>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            toc.WriteAllText(html.ToString());
        }

        #endregion
    }
}
